using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Graphics_project1
{
    internal static class PointPosition
    {
        private static int VertexRadius = 5;
        public static bool IsOnEdge(Point point, Vertex v1, Vertex v2)
        {
            int tolerance = 10;
            double edgeLength = Distance(v1.position, v2.position);

            double distanceToVertex1 = Distance(point, v1.position);
            double distanceToVertex2 = Distance(point, v2.position);

            return Math.Abs((distanceToVertex1 + distanceToVertex2) - edgeLength) <= tolerance;

            double Distance(Point p1, Point p2)
            {
                int dx = p1.X - p2.X;
                int dy = p1.Y - p2.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

        }

        public static bool IsOnVertex(Point point, Vertex vertex)
        {
            int dx = point.X - vertex.position.X;
            int dy = point.Y - vertex.position.Y;
            return (dx * dx + dy * dy) <= (VertexRadius * VertexRadius);
        }

        public static bool IsOnControl(Point point, RefPoint control)
        {
            int dx = point.X - control.X;
            int dy = point.Y - control.Y;
            return (dx * dx + dy * dy) <= (VertexRadius * VertexRadius);
        }


        public static bool IsInsidePolygon(Point point, Polygon polygon)
        {
            bool result = false;
            Vertex curr = polygon.head; 

            if (curr == null || curr.next == null) 
            {
                return false;
            }

            Vertex next = curr.next;

            do
            {
                next = curr.next != null ? curr.next : polygon.head;

                if ((curr.position.Y < point.Y && next.position.Y >= point.Y) ||
                    (next.position.Y < point.Y && curr.position.Y >= point.Y))
                {
                    if (curr.position.X + (point.Y - curr.position.Y) / (double)(next.position.Y - curr.position.Y) * (next.position.X - curr.position.X) < point.X)
                    {
                        result = !result; 
                    }
                }

                curr = next; 

            } while (curr != polygon.head); 

            return result;
        }

    }
}
