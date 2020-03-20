using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.Pathfinding;

namespace Sor.Game.Map {
    public class StructuralNavigationGraph : IAstarGraph<StructuralNavigationGraph.Node> {
        public const int DOOR_NODE_DIST = 2;
        public List<Node> nodes;

        public StructuralNavigationGraph(List<Node> nodes) {
            this.nodes = nodes;
        }
        
        public class Node {
            public Point pos;
            public List<Node> links = new List<Node>();
            public RoomLink edge;
            public Map.Room room;

            public Node(Point pos) {
                this.pos = pos;
            }

            public override string ToString() {
                return $"Node({pos})";
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

        public IEnumerable<Node> GetNeighbors(Node node) {
            return node.links;
        }

        public int Cost(Node src, Node dest) {
            // TODO: figure out a better way to do this
            // for now, base it on room center proximity
            var dist = PointExt.mhDist(src.pos, dest.pos);
            return dist;
        }

        public int Heuristic(Node node, Node goal) {
            return 0;
        }
    }
}