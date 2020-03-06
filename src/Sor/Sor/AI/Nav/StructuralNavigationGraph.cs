using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Sor.AI.Nav {
    public class StructuralNavigationGraph {
        public class Node {
            public Point pos;
            public List<Node> links;

            public Node(Point pos) {
                this.pos = pos;
            }
        }
    }
}