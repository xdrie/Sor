using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Sor.AI.Nav {
    public class StructuralNavigationGraph {
        public const int DOOR_NODE_DIST = 2;
        public class Node {
            public Point pos;
            public List<Node> links;

            public Node(Point pos) {
                this.pos = pos;
            }
        }
    }
}