using Microsoft.Xna.Framework;

namespace Sor.AI.Nav {
    public class StructuralNavigationGraph {
        public struct Node {
            public Point pos;

            public Node(Point pos) {
                this.pos = pos;
            }
        }
    }
}