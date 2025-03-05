using Microsoft.VisualBasic.Devices;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;


namespace Graphics_project1
{
    public partial class PolygonEditor : Form
    {

        private Bitmap _canvas;
        private Polygon polygon;
        private string mode = "";
        private Vertex movingVertex = null;
        private bool isVertexMoving = false;
        private bool isPolygonMoving = false;
        private Point _previousMouseLocation;
        private ContextMenuStrip contextMenuStrip;
        private ContextMenuStrip contextMenuStripVertex;
        private Vertex constraintEdgeStart;
        private Vertex constraintEdgeEnd;
        private List<EdgeConstraint> edgeConstraints;
        private List<Bazier> edgeBazier;
        private object contstraintEdge;
        private RefPoint movingControl = null;
        private bool isControlMoving = false;
        private Vertex vertexUnderAction = null;
        private bool isControlOk;

        public PolygonEditor()
        {
            InitializeComponent();

            contextMenuStrip = new ContextMenuStrip();
            contextMenuStripVertex = new ContextMenuStrip();

            contextMenuStrip.Items.Add("Horizontal", null, OnHorizontalClicked);
            contextMenuStrip.Items.Add("Vertical", null, OnVerticalClicked);
            contextMenuStrip.Items.Add("Costant Length", null, OnConstantClicked);
            contextMenuStrip.Items.Add("None", null, OnNoneClicked);
            contextMenuStrip.Items.Add("Turn on/turn off Bazier's segment", null, OnBazierClicked);

            contextMenuStripVertex.Items.Add("Delete Vertex", null, OnDeleteVertex);
            contextMenuStripVertex.Items.Add("Set G0", null, OnG0);
            contextMenuStripVertex.Items.Add("Set G1", null, OnG1);
            contextMenuStripVertex.Items.Add("Set C1", null, OnC1);
            InicilizeDemoPolygon();

        }

        private void OnC1(object? sender, EventArgs e)
        {
            vertexUnderAction.Continousity = "C1";

            UpdateCanvas();
        }

        private void OnG1(object? sender, EventArgs e)
        {

            vertexUnderAction.Continousity = "G1";
            UpdateCanvas();
        }

        private void OnG0(object? sender, EventArgs e)
        {

            vertexUnderAction.Continousity = "G0";
            UpdateCanvas();
        }

        private void OnDeleteVertex(object? sender, EventArgs e)
        {
            foreach (var c in edgeConstraints)
            {
                if (c.EdgeStart == vertexUnderAction)
                {
                    edgeConstraints.Remove(c);
                    break;
                }
            }

            foreach (var c in edgeConstraints)
            {
                if (c.EdgeEnd == vertexUnderAction)
                {
                    edgeConstraints.Remove(c);
                    break;
                }
            }
            if (polygon.head == vertexUnderAction && polygon.tail == vertexUnderAction)
            {
                polygon.head = null;
                polygon.tail = null;
            }
            else if (polygon.head == vertexUnderAction)
            {
                polygon.head = vertexUnderAction.next; 
                if (polygon.head != null)
                {
                    polygon.head.prev = null; 
                }
            }
            else if (polygon.tail == vertexUnderAction)
            {
                polygon.tail = vertexUnderAction.prev; 
                polygon.tail.next = null; 
            }

            else
            {
                Vertex prev = vertexUnderAction.prev;
                Vertex next = vertexUnderAction.next;
                prev.next = next; 
                if (next != null) next.prev = prev; 
            }

            polygon.vertexes.Remove(vertexUnderAction);

            UpdateCanvas();
            MessageBox.Show("Wierzchołek został usunięty.");
        }

        private void OnBazierClicked(object? sender, EventArgs e)
        {
            foreach (EdgeConstraint c in edgeConstraints)
            {
                if ((c.EdgeEnd == constraintEdgeEnd && c.EdgeStart == constraintEdgeStart) || (c.EdgeStart == constraintEdgeEnd && c.EdgeEnd == constraintEdgeStart))
                {
                    edgeConstraints.Remove(c);
                    break;
                }
            }

            foreach (var edge in edgeBazier)
            {
                if ((edge.Vertex1 == constraintEdgeEnd && edge.Vertex2 == constraintEdgeStart) || (edge.Vertex1 == constraintEdgeStart && edge.Vertex2 == constraintEdgeEnd))
                {
                    edgeBazier.Remove(edge);

                    UpdateCanvas();
                    return;
                }
            }

            int c1x = Math.Min(constraintEdgeEnd.position.X, constraintEdgeStart.position.X) + Math.Abs(constraintEdgeEnd.position.X - constraintEdgeStart.position.X) / 3;
            int c1y = Math.Min(constraintEdgeEnd.position.Y, constraintEdgeStart.position.Y) + Math.Abs(constraintEdgeEnd.position.Y - constraintEdgeStart.position.Y) / 3;
            RefPoint c1 = new RefPoint(c1x - 10, c1y - 10);

            int c2x = Math.Min(constraintEdgeEnd.position.X, constraintEdgeStart.position.X) + Math.Abs(constraintEdgeEnd.position.X - constraintEdgeStart.position.X) * 2 / 3;
            int c2y = Math.Min(constraintEdgeEnd.position.Y, constraintEdgeStart.position.Y) + Math.Abs(constraintEdgeEnd.position.Y - constraintEdgeStart.position.Y) * 2 / 3;
            RefPoint c2 = new RefPoint(c2x + 20, c2y + 10);

            if (constraintEdgeStart.next != constraintEdgeEnd && constraintEdgeStart != polygon.tail)
            {
                var temp = constraintEdgeStart;
                constraintEdgeStart = constraintEdgeEnd;
                constraintEdgeEnd = temp;
            }

            edgeBazier.Add(new Bazier(constraintEdgeStart, constraintEdgeEnd, c1, c2));
            using (Graphics g = Graphics.FromImage(_canvas))
            {
                g.Clear(Color.White);
            }
            Draw.DrawShape(_canvas, polygon, edgeConstraints, edgeBazier, out isControlOk);
            pictureBox1.Refresh();
        }

        private void OnNoneClicked(object? sender, EventArgs e)
        {
            foreach (EdgeConstraint c in edgeConstraints)
            {
                if ((c.EdgeEnd == constraintEdgeEnd && c.EdgeStart == constraintEdgeStart) || (c.EdgeStart == constraintEdgeEnd && c.EdgeEnd == constraintEdgeStart))
                {
                    edgeConstraints.Remove(c);
                    break;
                }
            }
            UpdateCanvas();
        }

        private void OnConstantClicked(object? sender, EventArgs e)
        {
            foreach (EdgeConstraint c in edgeConstraints)
            {
                if ((c.EdgeEnd == constraintEdgeEnd && c.EdgeStart == constraintEdgeStart) || (c.EdgeStart == constraintEdgeEnd && c.EdgeEnd == constraintEdgeStart))
                {
                    edgeConstraints.Remove(c);
                    break;
                }
            }
            string result = InputBox.Show("length", "Choose a canstant length:");

            int n;
            if (!string.IsNullOrEmpty(result) && int.TryParse(result, out n))
            {
                MessageBox.Show("Chosen length: " + result);
                edgeConstraints.Add(new EdgeConstraint(constraintEdgeEnd, constraintEdgeStart, ConstraintType.FixedLength, n));
                int currN = (int)(Math.Pow(constraintEdgeEnd.position.X - constraintEdgeStart.position.X, 2) +
                  Math.Pow(constraintEdgeEnd.position.Y - constraintEdgeStart.position.Y, 2));

                int diff = n - currN;

                Vertex smaller = constraintEdgeStart;
                Vertex larger = constraintEdgeEnd;
                if (constraintEdgeEnd.position.X < smaller.position.X)
                {
                    smaller = constraintEdgeEnd;
                    larger = constraintEdgeStart;
                }

                if (diff != 0)
                {
                    double adjustmentFactor = Math.Sqrt(Math.Abs(diff)) / Math.Sqrt(currN);

                    int direction = diff > 0 ? -1 : 1;

                    smaller.position.X += direction * (int)((larger.position.X - smaller.position.X) * adjustmentFactor);
                    smaller.position.Y += direction * (int)((larger.position.Y - smaller.position.Y) * adjustmentFactor);
                }

                CorrectEdges();

            }
            else
            {
                MessageBox.Show("Incorrect value");
            }
            CorrectEdges();
            UpdateCanvas();
        }

        private void OnVerticalClicked(object? sender, EventArgs e)
        {
            foreach (EdgeConstraint c in edgeConstraints)
            {
                if ((c.EdgeEnd == constraintEdgeEnd && c.EdgeStart == constraintEdgeStart) || (c.EdgeStart == constraintEdgeEnd && c.EdgeEnd == constraintEdgeStart))
                {
                    edgeConstraints.Remove(c);
                    break;
                }
            }

            foreach (EdgeConstraint c in edgeConstraints)
            {
                if (c.Type == ConstraintType.Vertical && (c.EdgeEnd == constraintEdgeEnd || c.EdgeEnd == constraintEdgeStart || c.EdgeStart == constraintEdgeEnd || c.EdgeStart == constraintEdgeStart))
                {
                    MessageBox.Show("Constarint not possible");
                    return;
                }
            }
            edgeConstraints.Add(new EdgeConstraint(constraintEdgeEnd, constraintEdgeStart, ConstraintType.Vertical));

            CorrectEdges();

            UpdateCanvas();
        }

        private void OnHorizontalClicked(object? sender, EventArgs e)
        {
            foreach (EdgeConstraint c in edgeConstraints)
            {
                if ((c.EdgeEnd == constraintEdgeEnd && c.EdgeStart == constraintEdgeStart) || (c.EdgeStart == constraintEdgeEnd && c.EdgeEnd == constraintEdgeStart))
                {
                    edgeConstraints.Remove(c);
                    break;
                }
            }
            foreach (EdgeConstraint c in edgeConstraints)
            {
                if (c.Type == ConstraintType.Horizontal && (c.EdgeEnd == constraintEdgeEnd || c.EdgeEnd == constraintEdgeStart || c.EdgeStart == constraintEdgeEnd || c.EdgeStart == constraintEdgeStart))
                {
                    MessageBox.Show("Constarint not possible");
                    return;
                }
            }
            edgeConstraints.Add(new EdgeConstraint(constraintEdgeEnd, constraintEdgeStart, ConstraintType.Horizontal));
            int newY = (constraintEdgeEnd.position.Y + constraintEdgeStart.position.Y) / 2;
            constraintEdgeEnd.position.Y = newY;
            constraintEdgeStart.position.Y = newY;
            if (constraintEdgeEnd.position.X == constraintEdgeStart.position.X)
            {
                constraintEdgeEnd.position.X += 5;
            }

            CorrectEdges();

            UpdateCanvas();
        }

        private void EditButton_CheckedChanged(object sender, EventArgs e)
        {
            if (EditButton.Checked)
            {
                if (mode != "edit")
                {
                    pictureBox1.MouseDown += PictureBox1_MouseDownEdit;
                    pictureBox1.MouseMove += PictureBox1_MouseMoveEdit;
                    pictureBox1.MouseUp += PictureBox1_MouseUpEdit;
                    mode = "edit";

                    label1.Text = "Click the right mouse button on the edge to add a vertex in the middle of it.\n" +
                        "Click the left mouse button on the edge to add constrains or Bezier curve.\n" +
                        "Click and hold left mouse button on the vertex to move it.\n" +
                        "Click the right mouse button on the vertex to change its continousity or delete it.\n" +
                        "Click and hold left mouse button on the inside of the polygon to move it.";
                }
            }
            else
            {
                if (mode == "edit")
                {
                    pictureBox1.MouseDown -= PictureBox1_MouseDownEdit;
                    pictureBox1.MouseMove -= PictureBox1_MouseMoveEdit;
                    pictureBox1.MouseUp -= PictureBox1_MouseUpEdit;
                }
            }
        }

        private void DeleteButton_CheckedChanged(object sender, EventArgs e)
        {
            edgeBazier.Clear();
            edgeConstraints.Clear();
            polygon = new Polygon();
            using (Graphics g = Graphics.FromImage(_canvas))
            {
                g.Clear(Color.White);
            }
            pictureBox1.Refresh();
            mode = "delete";

            label1.Text = "Polygon deleted";
        }

        private void AddButton_CheckedChanged(object sender, EventArgs e)
        {
            if (AddButton.Checked)
            {
                if (mode != "add")
                {
                    edgeBazier = new();
                    polygon = new Polygon();
                    edgeConstraints = new();
                    using (Graphics g = Graphics.FromImage(_canvas))
                    {
                        g.Clear(Color.White);
                    }
                    pictureBox1.Refresh();

                    pictureBox1.MouseDown += PictureBox1_MouseDownAdd;
                    label1.Text = "Click the location You want to place the next vertex at";
                    mode = "add";
                }
            }
            else
            {
                if (mode == "add")
                {
                    pictureBox1.MouseDown -= PictureBox1_MouseDownAdd;
                }
            }
        }

        private void PictureBox1_MouseDownAdd(object sender, MouseEventArgs e)
        {
            Vertex newVertex = new Vertex(new Point(e.Location.X, e.Location.Y), null, polygon.tail);
            if (polygon.vertexes.Count == 0)
            {
                polygon.head = newVertex;
                polygon.tail = newVertex;
                polygon.vertexes.Add(newVertex);
            }
            else
            {
                polygon.tail.next = newVertex;
                polygon.vertexes.Add(newVertex);
                polygon.tail = newVertex;
            }
            UpdateCanvas();
        }

        private void PictureBox1_MouseDownEdit(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Vertex curr = polygon.head;
                bool foundVertex = false;
                bool pointFound = false;

                while (curr != null)
                {
                    if (PointPosition.IsOnVertex(e.Location, curr))
                    {
                        pointFound = true;
                        foundVertex = true;
                        bool brazier = false;

                        foreach (var b in edgeBazier)
                        {
                            if (b.Vertex1 == curr || b.Vertex2 == curr)
                            {
                                vertexUnderAction = curr;
                                contextMenuStripVertex.Show(pictureBox1, e.Location);
                                brazier = true;
                                pointFound = true;
                                break;
                            }
                        }

                        if (!brazier)
                        {
                            DialogResult result = MessageBox.Show("Czy na pewno chcesz usunąć ten wierzchołek?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                foreach (var c in edgeConstraints)
                                {
                                    if (c.EdgeStart == curr)
                                    {
                                        edgeConstraints.Remove(c);
                                        break;
                                    }
                                }

                                foreach (var c in edgeConstraints)
                                {
                                    if (c.EdgeEnd == curr)
                                    {
                                        edgeConstraints.Remove(c);
                                        break;
                                    }
                                }
                                if (polygon.head == curr && polygon.tail == curr)
                                {
                                    polygon.head = null;
                                    polygon.tail = null;
                                }
                                else if (polygon.head == curr)
                                {
                                    polygon.head = curr.next; 
                                    if (polygon.head != null)
                                    {
                                        polygon.head.prev = null; 
                                    }
                                }
                                else if (polygon.tail == curr)
                                {
                                    polygon.tail = curr.prev; 
                                    polygon.tail.next = null; 
                                }

                                else
                                {
                                    Vertex prev = curr.prev;
                                    Vertex next = curr.next;
                                    prev.next = next; 
                                    if (next != null) next.prev = prev; 
                                }

                                polygon.vertexes.Remove(curr);

                                UpdateCanvas();
                                MessageBox.Show("Wierzchołek został usunięty.");
                            }
                            else
                            {
                                MessageBox.Show("Operacja została anulowana.");
                            }
                        }
                        break;
                    }
                    curr = curr.next;
                }

                curr = polygon.head;
                while (!pointFound && curr.next != null)
                {
                    Vertex next = curr.next;
                    if (PointPosition.IsOnEdge(e.Location, curr, next))
                    {
                        DialogResult result = MessageBox.Show("Czy na pewno chcesz dodać wierzchołek?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        pointFound = true;

                        if (result == DialogResult.Yes)
                        {
                            foreach (var c in edgeConstraints)
                            {
                                if (c.EdgeStart == curr && c.EdgeEnd == next)
                                {
                                    edgeConstraints.Remove(c);
                                    break;
                                }
                            }

                            foreach (var c in edgeConstraints)
                            {
                                if (c.EdgeEnd == curr && c.EdgeStart == next)
                                {
                                    edgeConstraints.Remove(c);
                                    break;
                                }
                            }
                            int newX = Math.Min(curr.position.X, next.position.X) + Math.Abs(curr.position.X - next.position.X) / 2;
                            int newY = Math.Min(curr.position.Y, next.position.Y) + Math.Abs(curr.position.Y - next.position.Y) / 2;
                            Point newVertexPosition = new Point(newX, newY);
                            Vertex newVertex = new Vertex(newVertexPosition, next, curr);


                            next.prev = newVertex;
                            curr.next = newVertex;
                            polygon.vertexes.Add(newVertex);

                            UpdateCanvas();
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Operacja została anulowana.");
                        }
                    }

                    curr = curr.next;
                }
                if (!pointFound && PointPosition.IsOnEdge(e.Location, polygon.tail, polygon.head))
                {
                    DialogResult result = MessageBox.Show("Czy na pewno chcesz dodać wierzchołek?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    pointFound = true;
                    if (result == DialogResult.Yes)
                    {
                        foreach (var c in edgeConstraints)
                        {
                            if (c.EdgeStart == polygon.tail && c.EdgeEnd == polygon.head)
                            {
                                edgeConstraints.Remove(c);
                                break;
                            }
                        }

                        foreach (var c in edgeConstraints)
                        {
                            if (c.EdgeEnd == polygon.tail && c.EdgeStart == polygon.head)
                            {
                                edgeConstraints.Remove(c);
                                break;
                            }
                        }
                        int newX = Math.Min(polygon.tail.position.X, polygon.head.position.X) + Math.Abs(polygon.tail.position.X - polygon.head.position.X) / 2;
                        int newY = Math.Min(polygon.tail.position.Y, polygon.head.position.Y) + Math.Abs(polygon.tail.position.Y - polygon.head.position.Y) / 2;
                        Point newVertexPosition = new Point(newX, newY);
                        Vertex newVertex = new Vertex(newVertexPosition, polygon.head, null);


                        polygon.head.prev = newVertex;
                        polygon.head = newVertex;
                        polygon.vertexes.Add(newVertex);

                        UpdateCanvas();
                    }
                    else
                    {
                        MessageBox.Show("Operacja została anulowana.");
                    }
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                bool pointFound = false;
                Vertex curr = polygon.head;
                while (curr != null)
                {
                    if (PointPosition.IsOnVertex(e.Location, curr))
                    {
                        movingVertex = curr;
                        isVertexMoving = true;
                        pointFound = true;
                        break;
                    }
                    curr = curr.next;
                }
                if (!pointFound)
                {
                    foreach (Bazier b in edgeBazier)
                    {
                        if (PointPosition.IsOnControl(e.Location, b.Control1))
                        {
                            isVertexMoving = false;
                            movingControl = b.Control1;
                            isControlMoving = true;
                            pointFound = true;
                            break;
                        }

                        if (PointPosition.IsOnControl(e.Location, b.Control2))
                        {
                            isVertexMoving = false;
                            movingControl = b.Control2;
                            isControlMoving = true;
                            pointFound = true;
                            break;
                        }
                    }
                }
                if (!pointFound && PointPosition.IsInsidePolygon(e.Location, polygon))
                {
                    _previousMouseLocation = e.Location;
                    isPolygonMoving = true;
                    pointFound = true;
                }

                curr = polygon.head;
                while (!pointFound && curr.next != null)
                {
                    Vertex next = curr.next;
                    if (PointPosition.IsOnEdge(e.Location, curr, next))
                    {
                        constraintEdgeStart = curr;
                        constraintEdgeEnd = next;
                        contextMenuStrip.Show(pictureBox1, e.Location);
                    }

                    curr = curr.next;
                }
                if (!pointFound && PointPosition.IsOnEdge(e.Location, polygon.head, polygon.tail))
                {
                    constraintEdgeEnd = polygon.head;
                    constraintEdgeStart = polygon.tail;
                    contextMenuStrip.Show(pictureBox1, e.Location);
                }

            }
        }
        private void PictureBox1_MouseMoveEdit(object sender, MouseEventArgs e)
        {
            if (movingVertex != null && isVertexMoving)
            {
                ConstraintType type = ConstraintType.None;
                foreach (EdgeConstraint c in edgeConstraints)
                {
                    if (c.EdgeEnd == movingVertex || c.EdgeStart == movingVertex && type == ConstraintType.None)
                    {
                        type = c.Type;
                    }
                }

                if (type == ConstraintType.None)
                {
                    movingVertex.position = e.Location;
                    UpdateCanvas();
                }
                else if (type == ConstraintType.Vertical && Math.Abs(movingVertex.position.X - e.Location.X) < 5)
                {
                    movingVertex.position.Y = e.Location.Y;
                    UpdateCanvas();
                }
                else if (type == ConstraintType.Horizontal && Math.Abs(movingVertex.position.Y - e.Location.Y) < 5)
                {
                    movingVertex.position.X = e.Location.X;
                    UpdateCanvas();
                }


                else if (type == ConstraintType.FixedLength)
                {
                    Vertex first = null;
                    Vertex second = null;
                    foreach (var c in edgeConstraints)
                    {
                        if (c.EdgeStart == movingVertex && c.Type == ConstraintType.FixedLength)
                        {
                            first = c.EdgeEnd;
                        }
                        if (c.EdgeEnd == movingVertex && c.Type == ConstraintType.FixedLength)
                        {
                            second = c.EdgeStart;
                        }
                    }
                    if (first != null && second == null && (first.position.X - movingVertex.position.X) * (first.position.X - movingVertex.position.X) +
                        (first.position.Y - movingVertex.position.Y) * (first.position.Y - movingVertex.position.Y) - ((first.position.X - e.Location.X) *
                        (first.position.X - e.Location.X) + (first.position.Y - e.Location.Y) * (first.position.Y - e.Location.Y)) == 0)
                    {
                        movingVertex.position.X = e.Location.X;
                        movingVertex.position.Y = e.Location.Y;
                    }
                    if (second != null && first == null && (second.position.X - movingVertex.position.X) * (second.position.X - movingVertex.position.X) +
                        (second.position.Y - movingVertex.position.Y) * (second.position.Y - movingVertex.position.Y) - ((second.position.X - e.Location.X) *
                        (second.position.X - e.Location.X) + (second.position.Y - e.Location.Y) * (second.position.Y - e.Location.Y)) == 0)
                    {
                        movingVertex.position.X = e.Location.X;
                        movingVertex.position.Y = e.Location.Y;
                    }
                    UpdateCanvas();
                }

            }
            else if (movingControl != null && isControlMoving)
            {
                movingControl.X = e.Location.X;
                movingControl.Y = e.Location.Y;

            }
            else if (isPolygonMoving)
            {

                int deltaX = e.Location.X - _previousMouseLocation.X;
                int deltaY = e.Location.Y - _previousMouseLocation.Y;
                Vertex curr = polygon.head;
                while (curr != null)
                {
                    curr.position.X += deltaX;
                    curr.position.Y += deltaY;
                    curr = curr.next;
                }
                foreach (Bazier b in edgeBazier)
                {
                    b.Control1.X += deltaX;
                    b.Control1.Y += deltaY;
                    b.Control2.X += deltaX;
                    b.Control2.Y += deltaY;
                }
                _previousMouseLocation = e.Location;
                UpdateCanvas();

            }
        }

        private void PictureBox1_MouseUpEdit(object sender, MouseEventArgs e)
        {
            //if (e.Button != MouseButtons.Left)
            {
                //movingVertex = null;
                isVertexMoving = false;
                isPolygonMoving = false;
                isControlMoving = false;

                CorrectEdges();
                UpdateCanvas();
            }
        }
        public void InicilizeDemoPolygon()
        {
            _canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = _canvas;
            polygon = new Polygon();
            polygon.LineDrawing = "own";
            mode = "";
            edgeConstraints = new List<EdgeConstraint>();
            edgeBazier = new List<Bazier>();
            Vertex v1 = new Vertex(new Point(100, 100), null, null);
            Vertex v2 = new Vertex(new Point(500, 100), null, v1);
            Vertex v3 = new Vertex(new Point(700, 500), null, v2);
            Vertex v4 = new Vertex(new Point(350, 500), null, v3);
            v1.next = v2;
            v2.next = v3;
            v3.next = v4;
            polygon.vertexes.Add(v1);
            polygon.vertexes.Add(v2);
            polygon.vertexes.Add(v3);
            polygon.vertexes.Add(v4);
            polygon.head = v1;
            polygon.tail = v4;
            UpdateCanvas();
            edgeConstraints.Add(new EdgeConstraint(v3, v4, ConstraintType.Horizontal));

            UpdateCanvas();
            v3.Continousity = "C1";
            edgeBazier.Add(new Bazier(v2, v3, new RefPoint(v2.position.X + 400, v2.position.Y + 100), new RefPoint(v2.position.X / 2 - 3000, v2.position.Y / 2 + 100)));

            UpdateCanvas();
        }

        public bool CorrectEdges()
        {
            Vertex current = polygon.head;
            while (current != polygon.tail)
            {
                Vertex next = current.next;
                EdgeConstraint constraint = edgeConstraints.Find(ec => (ec.EdgeStart == current && ec.EdgeEnd == next) || (ec.EdgeStart == next && ec.EdgeEnd == current));

                if (constraint != null && constraint.Type != ConstraintType.None)
                {
                    float currentLength = (float)Math.Sqrt((next.position.X - current.position.X) * (next.position.X - current.position.X) +
                                                            (next.position.Y - current.position.Y) * (next.position.Y - current.position.Y));

                    switch (constraint.Type)
                    {
                        case ConstraintType.Horizontal:
                            next.position.Y = current.position.Y;
                            if (current.position.X == next.position.X)
                            {
                                next.position.X += 5;
                            }
                            break;

                        case ConstraintType.Vertical:
                            next.position.X = current.position.X;

                            if (current.position.Y == next.position.Y)
                            {
                                next.position.Y += 5;
                            }
                            break;

                        case ConstraintType.FixedLength:
                            int desiredLength = (int)constraint.len;
                            if (currentLength != desiredLength)
                            {
                                float scale = desiredLength / currentLength;

                                next.position.X = (int)Math.Round(current.position.X + (next.position.X - current.position.X) * scale);
                                next.position.Y = (int)Math.Round(current.position.Y + (next.position.Y - current.position.Y) * scale);
                            }
                            break;
                    }
                }

                current = next;
            }

            EdgeConstraint lastConstraint = edgeConstraints.Find(ec => (ec.EdgeStart == polygon.head && ec.EdgeEnd == polygon.tail) || (ec.EdgeStart == polygon.tail && ec.EdgeEnd == polygon.head));
            if (lastConstraint == null || lastConstraint.Type == ConstraintType.None) { return true; }

            switch (lastConstraint.Type)
            {
                case ConstraintType.Horizontal:
                    if (polygon.head.position.Y != polygon.tail.position.Y)
                    {
                        polygon.head.position.Y = polygon.tail.position.Y;
                        if (polygon.head.position.X == polygon.tail.position.X)
                        {
                            polygon.head.position.X += 5;
                        }
                    }
                    break;

                case ConstraintType.Vertical:
                    if (polygon.head.position.X != polygon.tail.position.X)
                    {
                        polygon.head.position.X = polygon.tail.position.X;
                        if (polygon.head.position.Y == polygon.tail.position.Y)
                        {
                            polygon.head.position.Y += 5;
                        }
                    }
                    break;

                case ConstraintType.FixedLength:
                    float currentLength = (float)Math.Sqrt((polygon.head.position.X - polygon.tail.position.X) * (polygon.head.position.X - polygon.tail.position.X) +
                                                            (polygon.head.position.Y - polygon.tail.position.Y) * (polygon.head.position.Y - polygon.tail.position.Y));

                    int? desiredLength = lastConstraint.len;
                    if (currentLength != desiredLength)
                    {
                        float scale = (float)(desiredLength / currentLength);

                        polygon.head.position.X = (int)Math.Round(polygon.head.position.X + (polygon.head.position.X - polygon.tail.position.X) * scale);
                        polygon.head.position.Y = (int)Math.Round(polygon.tail.position.Y + (polygon.head.position.Y - polygon.tail.position.Y) * scale);

                    }
                    break;
            }


            EdgeConstraint constraint1 = edgeConstraints.Find(ec => (ec.EdgeEnd == polygon.head && ec.EdgeStart == polygon.head.next) || (ec.EdgeStart == polygon.head && ec.EdgeEnd == polygon.head.next));


            if (constraint1 != null && constraint1.Type != ConstraintType.None)
            {
                float currentLength = (float)Math.Sqrt((constraint1.EdgeEnd.position.X - constraint1.EdgeStart.position.X) * (constraint1.EdgeEnd.position.X - constraint1.EdgeStart.position.X) +
                                                        (constraint1.EdgeEnd.position.Y - constraint1.EdgeStart.position.Y) * (constraint1.EdgeEnd.position.Y - constraint1.EdgeStart.position.Y));

                switch (constraint1.Type)
                {
                    case ConstraintType.Horizontal:
                        if (constraint1.EdgeStart.position.Y != constraint1.EdgeEnd.position.Y)
                        {
                            //MessageBox.Show("Constraints impossible");
                            return false;
                        }
                        break;

                    case ConstraintType.Vertical:
                        if (constraint1.EdgeStart.position.X != constraint1.EdgeEnd.position.X)
                        {
                            //MessageBox.Show("Constraints impossible");
                            return false;
                        }

                        break;

                    case ConstraintType.FixedLength:
                        int desiredLength = (int)constraint1.len;
                        if (currentLength != desiredLength)
                        {
                            //MessageBox.Show("Constraints impossible");
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                polygon.LineDrawing = "own";
            }
            else
            {
                polygon.LineDrawing = "biblio";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                polygon.LineDrawing = "biblio";
            }
            else
            {
                polygon.LineDrawing = "own";
            }
        }

        private void UpdateCanvas()
        {
            using (Graphics g = Graphics.FromImage(_canvas))
            {
                g.Clear(Color.White);
            }

            Draw.DrawShape(_canvas, polygon, edgeConstraints, edgeBazier, out isControlOk);
            pictureBox1.Refresh();
        }

    }
}
