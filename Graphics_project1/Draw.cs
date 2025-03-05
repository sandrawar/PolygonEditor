using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics_project1
{
    internal static class Draw
    {
        public static void DrawVertex(this Bitmap canvas, Point A, int radius, Color color)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.FillEllipse(new SolidBrush(color), A.X - radius, A.Y - radius, radius * 2, radius * 2);
            }
        }
        // Algorytm Bresenhama do rysowania linii
        public static void DrawLine(this Bitmap canvas, Point A, Point B, Color color)
        {
            int width = B.X - A.X;
            int height = B.Y - A.Y;
            Point d1 = new Point(Math.Sign(width), Math.Sign(height));

            Point d2;
            int longerDim = Math.Abs(width);
            int shorterDim = Math.Abs(height);
            if (longerDim < shorterDim)
            {
                (longerDim, shorterDim) = (shorterDim, longerDim);
                d2 = new Point(0, d1.Y);
            }
            else
            {
                d2 = new Point(d1.X, 0);
            }

            int numerator = longerDim >> 1;

            for (int i = 0; i <= longerDim; ++i)
            {
                // Sprawdzenie, czy punkty są w granicach bitmapy
                if (A.X >= 0 && A.Y >= 0 && A.X < canvas.Width && A.Y < canvas.Height)
                {
                    canvas.SetPixel(A.X, A.Y, color);
                }

                numerator += shorterDim;
                if (numerator >= longerDim)
                {
                    numerator -= longerDim;
                    A.Offset(d1);
                }
                else
                {
                    A.Offset(d2);
                }
            }
        }

        public static void DrawShape(Bitmap canvas, Polygon polygon, List<EdgeConstraint> edgeConstraints, List<Bazier> bazierEdges, out bool isControlOk)
        {
            isControlOk = true;
            foreach (Vertex v in polygon.vertexes)
            {
                Draw.DrawVertex(canvas, v.position, 5, Color.Red);
            }

            Vertex curr = polygon.head;

            while (curr != polygon.tail)
            {
                bool blazier = false;
                foreach (Bazier b in bazierEdges)
                {
                    if ((b.Vertex1 == curr && b.Vertex2 == curr.next)) //|| (b.Vertex1 == curr.next && b.Vertex2 == curr))
                    {
                        Bazier? b2 = bazierEdges.Find(ba => ba.Vertex1 == curr.next);
                        Bazier? b3 = bazierEdges.Find(ba => ba.Vertex2 == curr);
                        using (Graphics g = Graphics.FromImage(canvas))
                        {
                            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 255));
                            RefPoint c1 = b.Control1;
                            RefPoint c2 = b.Control2;
                            Point p4 = b.Vertex2.next == null ? polygon.head.position : b.Vertex2.next.position;
                            Point p01 = b.Vertex1.prev == null ? polygon.tail.position : b.Vertex1.prev.position;
                            if(b2 != null) { p4 = new Point(b2.Control1.X, b2.Control1.Y); }
                            if (b3 != null) { p01 = new Point(b3.Control2.X, b3.Control2.Y); }
                            Draw.DrawBezierIncremental(g, pen, p01, b.Vertex1.position, new Point(b.Control1.X, b.Control1.Y), new Point(b.Control2.X, b.Control2.Y), b.Vertex2.position, p4, 1000, b.Vertex1.Continousity, b.Vertex2.Continousity, out isControlOk, out c1, out c2, polygon);
                            
                            {
                                b.Control1 = c1;
                                b.Control2 = c2;
                            }

                            Draw.DrawVertex(canvas, new Point(b.Control1.X, b.Control1.Y), 5, Color.Blue);
                            Draw.DrawVertex(canvas, new Point(b.Control2.X, b.Control2.Y), 5, Color.Blue);
                            pen = new Pen(Color.Blue, 1);
                            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                            g.DrawLine(pen, b.Vertex1.position, new Point(b.Control1.X, b.Control1.Y));
                            g.DrawLine(pen, b.Vertex2.position, new Point(b.Control2.X, b.Control2.Y));
                            g.DrawLine(pen, new Point(b.Control1.X, b.Control1.Y), new Point(b.Control2.X, b.Control2.Y));

                        }

                        blazier = true;
                        break;
                    }
                }
                if (!blazier)
                {
                    if (polygon.LineDrawing == "own")
                    {
                        //MessageBox.Show("own");
                        Draw.DrawLine(canvas, curr.position, curr.next.position, Color.Black);
                    }
                    else
                    {
                        //MessageBox.Show("biblio");
                        using (Graphics g = Graphics.FromImage(canvas))
                        {
                            Pen pen = new Pen(Color.Black, 1);
                            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                            g.DrawLine(pen, curr.position, curr.next.position);
                        }
                    }
                }
                curr = curr.next;
            }

            if (polygon.vertexes.Count > 1)
            {
                bool blazier = false;
                foreach (Bazier b in bazierEdges)
                {
                    if ((b.Vertex1 == polygon.tail && b.Vertex2 == polygon.head)) //|| (b.Vertex1 == polygon.head && b.Vertex2 == polygon.tail))
                    {

                        using (Graphics g = Graphics.FromImage(canvas))
                        {
                            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 255));
                            RefPoint c1 = b.Control1;
                            RefPoint c2 = b.Control2;
                            Draw.DrawBezierIncremental(g, pen, b.Vertex1.prev.position, b.Vertex1.position, new Point(b.Control1.X, b.Control1.Y), new Point(b.Control2.X, b.Control2.Y), b.Vertex2.position, b.Vertex2.next.position, 1000, b.Vertex1.Continousity, b.Vertex2.Continousity, out isControlOk, out c1, out c2, polygon);
                            
                            {
                                b.Control1 = c1;
                                b.Control2 = c2;
                            }

                            Draw.DrawVertex(canvas, new Point(b.Control1.X, b.Control1.Y), 5, Color.Blue);
                            Draw.DrawVertex(canvas, new Point(b.Control2.X, b.Control2.Y), 5, Color.Blue);
                            pen = new Pen(Color.Blue, 1);
                            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                            g.DrawLine(pen, b.Vertex1.position, new Point(b.Control1.X, b.Control1.Y));
                            g.DrawLine(pen, b.Vertex2.position, new Point(b.Control2.X, b.Control2.Y));
                            g.DrawLine(pen, new Point(b.Control1.X, b.Control1.Y), new Point(b.Control2.X, b.Control2.Y));

                        }
                        blazier = true;
                        break;
                    }
                }
                if (!blazier)
                {
                    Draw.DrawLine(canvas, polygon.head.position, polygon.tail.position, Color.Black);
                }
            }

            Draw.DrawConstraints(canvas, edgeConstraints);
            Draw.DrawContinousity(canvas, bazierEdges, polygon);

        }

        private static void DrawContinousity(Bitmap canvas, List<Bazier> bazierEdges, Polygon polygon)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                foreach (var b in bazierEdges)
                {
                    switch (b.Vertex1.Continousity)
                    {
                        case "G0":
                            g.DrawString("G0", new Font("Arial", 8), Brushes.Black, b.Vertex1.position.X + 5, b.Vertex1.position.Y + 5);
                            break;
                        case "C1":
                            g.DrawString("C1", new Font("Arial", 8), Brushes.Black, b.Vertex1.position.X + 5, b.Vertex1.position.Y + 5);
                            break;
                        case "G1":
                            g.DrawString("G1", new Font("Arial", 8), Brushes.Black, b.Vertex1.position.X + 5, b.Vertex1.position.Y + 5);
                            break;
                    }

                    switch (b.Vertex2.Continousity)
                    {
                        case "G0":
                            g.DrawString("G0", new Font("Arial", 8), Brushes.Black, b.Vertex2.position.X + 5, b.Vertex2.position.Y + 5);
                            break;
                        case "C1":
                            g.DrawString("C1", new Font("Arial", 8), Brushes.Black, b.Vertex2.position.X + 5, b.Vertex2.position.Y + 5);
                            break;
                        case "G1":
                            g.DrawString("G1", new Font("Arial", 8), Brushes.Black, b.Vertex2.position.X + 5, b.Vertex2.position.Y + 5);
                            break;
                    }
                }
            }
        }

        public static void DrawConstraints(this Bitmap canvas, List<EdgeConstraint> edgeConstraints)
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                foreach (var constraint in edgeConstraints)
                {
                    // Narysuj ikonkę ograniczenia w środku krawędzi
                    int centerX = (constraint.EdgeStart.position.X + constraint.EdgeEnd.position.X) / 2;
                    int centerY = (constraint.EdgeStart.position.Y + constraint.EdgeEnd.position.Y) / 2;

                    switch (constraint.Type)
                    {
                        case ConstraintType.Horizontal:
                            g.DrawString("H", new Font("Arial", 12), Brushes.Black, centerX, centerY);
                            break;
                        case ConstraintType.Vertical:
                            g.DrawString("V", new Font("Arial", 12), Brushes.Black, centerX, centerY);
                            break;
                        case ConstraintType.FixedLength:
                            g.DrawString("L", new Font("Arial", 12), Brushes.Black, centerX, centerY);
                            break;
                    }
                }
            }
        }

        public static void DrawBezierIncremental(
        Graphics g,
        Pen pen,
        PointF p_01, 
        PointF p0,  
        PointF p1,  // Control point 1
        PointF p2,  // Control point 2
        PointF p3,  
        PointF p4,  
        int steps,
        string continuityStart, 
        string continuityEnd,   
        out bool isControlOk,
        out RefPoint Control1,
        out RefPoint Control2,
        Polygon polygon)
        {
            isControlOk = true;
            Control1 = new RefPoint((int)Math.Round(p1.X), (int)Math.Round(p1.Y));
            Control2 = new RefPoint((int)Math.Round(p2.X), (int)Math.Round(p2.Y));

            float step = 1f / steps;

            if (continuityStart == "G1" || continuityStart == "C1")
            {
                PointF directionStart = new PointF(p0.X - p_01.X, p0.Y - p_01.Y);

                float lengthStart = (float)Math.Sqrt(directionStart.X * directionStart.X + directionStart.Y * directionStart.Y);
                if (lengthStart > 0)
                {
                    directionStart.X /= lengthStart;
                    directionStart.Y /= lengthStart;
                }

                if (continuityStart == "G1")
                {
                    float p0ToP1Length = (float)Math.Sqrt((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y));

                    p1 = new PointF(p0.X + directionStart.X * p0ToP1Length, p0.Y + directionStart.Y * p0ToP1Length);

                    Control1 = new RefPoint((int)Math.Round(p1.X), (int)Math.Round(p1.Y));
                }

                if (continuityStart == "C1")
                {
                    float p0ToP1Length = lengthStart / 3;

                    if (polygon.vertexes.Find(v => v.position.X == p_01.X && v.position.Y == p_01.Y) == null) { p0ToP1Length *= 3; }

                    p1 = new PointF(p0.X + directionStart.X * p0ToP1Length, p0.Y + directionStart.Y * p0ToP1Length);

                    Control1 = new RefPoint((int)Math.Round(p1.X), (int)Math.Round(p1.Y));
                }
            }


            if (continuityEnd == "G1" || continuityEnd == "C1")
            {
                PointF directionEnd = new PointF(p4.X - p3.X, p4.Y - p3.Y);

                float lengthEnd = (float)Math.Sqrt(directionEnd.X * directionEnd.X + directionEnd.Y * directionEnd.Y);
                if (lengthEnd > 0)
                {
                    directionEnd.X /= lengthEnd;
                    directionEnd.Y /= lengthEnd;
                }

                if (continuityEnd == "G1")
                {
                    float p3ToP2Length = (float)Math.Sqrt((p2.X - p3.X) * (p2.X - p3.X) + (p2.Y - p3.Y) * (p2.Y - p3.Y));

                    p2 = new PointF(p3.X - directionEnd.X * p3ToP2Length, p3.Y - directionEnd.Y * p3ToP2Length);

                    Control2 = new RefPoint((int)Math.Round(p2.X), (int)Math.Round(p2.Y));
                }

                if (continuityEnd == "C1")
                {
                    float p3ToP2Length = lengthEnd / 3;
                    if(polygon.vertexes.Find(v => v.position.X == p4.X && v.position.Y == p4.Y) == null) { p3ToP2Length *= 3; }

                    p2 = new PointF(p3.X - directionEnd.X * p3ToP2Length, p3.Y - directionEnd.Y * p3ToP2Length);

                    Control2 = new RefPoint((int)Math.Round(p2.X), (int)Math.Round(p2.Y));
                }

            }

            PointF prevPoint = p0;

            for (int i = 1; i <= steps; i++)
            {
                float t = i * step; 

                float x = (float)(Math.Pow(1 - t, 3) * p0.X +
                                  3 * Math.Pow(1 - t, 2) * t * p1.X +
                                  3 * (1 - t) * Math.Pow(t, 2) * p2.X +
                                  Math.Pow(t, 3) * p3.X);

                float y = (float)(Math.Pow(1 - t, 3) * p0.Y +
                                  3 * Math.Pow(1 - t, 2) * t * p1.Y +
                                  3 * (1 - t) * Math.Pow(t, 2) * p2.Y +
                                  Math.Pow(t, 3) * p3.Y);

                PointF currPoint = new PointF(x, y);

                g.DrawLine(pen, prevPoint, currPoint);

                prevPoint = currPoint;
            }
        }



    }

}

