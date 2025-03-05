using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics_project1
{ 
    public enum ConstraintType
    {
        None,
        Horizontal,
        Vertical,
        FixedLength
    }

    public class EdgeConstraint
    {
        public Vertex EdgeStart { get; set; }
        public Vertex EdgeEnd { get; set; }
        public ConstraintType Type { get; set; }
        public int? len { get; set; }

        public EdgeConstraint(Vertex start, Vertex end, ConstraintType type, int? len = null)
        {
            EdgeStart = start;
            EdgeEnd = end;
            Type = type;
            this.len = len;
        }
    }

    public class Bazier
    {
        public Vertex Vertex1 { get; set; }
        public Vertex Vertex2 { get; set; }
        public RefPoint Control1 { get; set; }
        public RefPoint Control2 { get; set; }
        public Bazier(Vertex v1, Vertex v2, RefPoint c1, RefPoint c2)
        {
            Vertex1 = v1;
            Vertex2 = v2;
            Control1 = c1;
            Control2 = c2;
        }
    }
}
