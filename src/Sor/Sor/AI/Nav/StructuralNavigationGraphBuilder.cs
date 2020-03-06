using System.Collections.Generic;
using Sor.Game;

namespace Sor.AI.Nav {
    public class StructuralNavigationGraphBuilder {
        private RoomGraph roomGraph;
        private Dictionary<Map.Room, StructuralNavigationGraph.Node> sngNodes;

        public StructuralNavigationGraphBuilder(RoomGraph roomGraph) {
            this.roomGraph = roomGraph;
        }

        public StructuralNavigationGraph build() {
            return null;
        }

        public void analyze() {
            // 1. build mapping from room to center nodes
            foreach (var room in roomGraph.rooms) {
                // create unconnected center nodes
                sngNodes[room] = new StructuralNavigationGraph.Node(room.center);
            }
            // 2. create nodes of indirection
        }
    }
}