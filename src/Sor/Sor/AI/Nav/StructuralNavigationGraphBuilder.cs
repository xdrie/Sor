using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sor.Game;
using Sor.Util;

namespace Sor.AI.Nav {
    public class StructuralNavigationGraphBuilder {
        private RoomGraph roomGraph;
        private Dictionary<Map.Room, DelayedNode> sngNodes;

        class DelayedNode {
            public StructuralNavigationGraph.Node centerNode;
            private List<DelayedNode> pendingLinks;

            public DelayedNode(StructuralNavigationGraph.Node centerNode) {
                this.centerNode = centerNode;
            }

            public void addPendingLink(DelayedNode node) {
                pendingLinks.Add(node);
            }

            public void collapse() {
                // recursively collapse all pending nodes
                foreach (var link in pendingLinks) {
                    link.collapse(); // collapse this child link
                    centerNode.links.Add(link.centerNode); // attach the child link to me (center)
                }
            }
        }

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
                sngNodes[room] = new DelayedNode(
                    new StructuralNavigationGraph.Node(room.center));
            }

            // 2. create nodes of indirection
            foreach (var nodePair in sngNodes) {
                var room = nodePair.Key;
                var centerNode = nodePair.Value;
                foreach (var door in room.doors) {
                    // calculate positions of inner and outer door nodes
                    var doorCenter = door.doorCenter;
                    var (dx, dy) = DirectionStepper.stepIn(door.dir);
                    var innerDoor = doorCenter + new Point(-dx * StructuralNavigationGraph.DOOR_NODE_DIST,
                        -dy * StructuralNavigationGraph.DOOR_NODE_DIST);
                    var outerDoor = doorCenter + new Point(dx * StructuralNavigationGraph.DOOR_NODE_DIST,
                        dy * StructuralNavigationGraph.DOOR_NODE_DIST);
                    var innerDoorNode = new DelayedNode(new StructuralNavigationGraph.Node(innerDoor));
                    var outerDoorNode = new DelayedNode(new StructuralNavigationGraph.Node(outerDoor));
                    centerNode.addPendingLink(innerDoorNode); // attach CENTER - INNER
                    innerDoorNode.addPendingLink(outerDoorNode); // attach INNER - OUTER
                    // when collapsed, we should get CENTER - INNER - OUTER (a "spike")
                }
            }
            // now, we have all our nodes of the form
            // CENTER with [INNER - OUTER] spikes for each door
            // next, we need to merge these spikes by door link
        }
    }
}