using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sor.Game;
using Sor.Util;

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
            foreach (var nodePair in sngNodes) {
                var room = nodePair.Key;
                var centerNode = nodePair.Value;
                foreach (var door in room.doors) {
                    // calculate positions of inner and outer door nodes
                    var doorCenter = door.doorCenter;
                    var (dx, dy) = DirectionStepper.stepIn(door.dir);
                    var outerDoor = doorCenter + new Point(dx * StructuralNavigationGraph.DOOR_NODE_DIST,
                        dy * StructuralNavigationGraph.DOOR_NODE_DIST);
                    var innerDoor = doorCenter + new Point(-dx * StructuralNavigationGraph.DOOR_NODE_DIST,
                        -dy * StructuralNavigationGraph.DOOR_NODE_DIST);
                }
            }
        }
    }
}