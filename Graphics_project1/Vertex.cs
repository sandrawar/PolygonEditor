using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics_project1
{
    public class Vertex
    {
        public Point position;
        public Vertex? next;
        public Vertex? prev;

        public string Continousity { get; set; }
        public Vertex(Point position, Vertex? next, Vertex? prev, string continuosity = "G1")
        {
            this.position = position;
            this.next = next;
            this.prev = prev;
            this.Continousity = continuosity;
        }
    }

    public class Polygon
    {
        public List<Vertex> vertexes = new List<Vertex>();
        public Vertex? head = null;
        public Vertex? tail = null;
        public string LineDrawing = "own";
    }

    public class RefPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public RefPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
