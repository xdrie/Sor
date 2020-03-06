using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sor.Game;

namespace Sor.AI.Nav {
    public class StructuralNavigationGraph {
        public const int DOOR_NODE_DIST = 2;
        public List<Node> nodes;

        public StructuralNavigationGraph(List<Node> nodes) {
            this.nodes = nodes;
        }
        
        public class Node {
            public Point pos;
            public List<Node> links;
            public RoomLink edge;

            public Node(Point pos) {
                this.pos = pos;
            }
        }
        
        public class RoomLink {
            public Map.Room r1;
            public Map.Room r2;

            public RoomLink(Map.Room r1, Map.Room r2) {
                this.r1 = r1;
                this.r2 = r2;
            }

            public bool equiv(RoomLink other) {
                return (r1 == other.r1 && r2 == other.r2) || (r1 == other.r2 && r2 == other.r1);
            }
        }
    }
}