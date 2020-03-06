using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sor.Game;
using Sor.Util;

namespace Sor.AI.Nav {
    public class StructuralNavigationGraphBuilder {
        private RoomGraph roomGraph;
        private Dictionary<Map.Room, DelayedNode> sngNodes = new Dictionary<Map.Room, DelayedNode>();

        class DelayedNode {
            public StructuralNavigationGraph.Node centerNode;
            private List<DelayedNode> pendingLinks = new List<DelayedNode>();

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
                    // attach the child link to me (center)
                    // (two-way)
                    centerNode.links.Add(link.centerNode); // add them to my links
                    link.centerNode.links.Add(centerNode); // add me to their links
                }
            }
        }

        public StructuralNavigationGraphBuilder(RoomGraph roomGraph) {
            this.roomGraph = roomGraph;
        }

        public StructuralNavigationGraph build() {
            var nodeList = sngNodes.Values.Select(x => x.centerNode).ToList();
            return new StructuralNavigationGraph(nodeList);
        }

        public void analyze() {
            // 1. build mapping from room to center nodes
            foreach (var room in roomGraph.rooms) {
                // create unconnected center nodes
                sngNodes[room] = new DelayedNode(new StructuralNavigationGraph.Node(room.center));
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
                    var outerDoorNode = new DelayedNode(new StructuralNavigationGraph.Node(outerDoor) {
                        edge = new StructuralNavigationGraph.RoomLink(door.roomLocal, door.roomOther)
                    });
                    centerNode.addPendingLink(innerDoorNode); // attach CENTER - INNER
                    innerDoorNode.addPendingLink(outerDoorNode); // attach INNER - OUTER
                    // when collapsed, we should get CENTER - INNER - OUTER (a "spike")
                }
            }

            // now, we have all our nodes of the form
            // CENTER with [INNER - OUTER] spikes for each door
            // 3. next, we need to merge these spikes by door link
            // and merge all these delayed nodes into a graph
            foreach (var nodePair in sngNodes) {
                var room = nodePair.Key;
                var centerNode = nodePair.Value;
                var gn = centerNode.centerNode;

                // do a BFS on the node. we are going to make a list of spike nodes.
                var spikeNodes = new List<StructuralNavigationGraph.Node>();
                var visited = new Dictionary<StructuralNavigationGraph.Node, bool>();
                var queue = new Queue<StructuralNavigationGraph.Node>();
                // mark the starting node as visited, and queue it
                visited[gn] = true;
                queue.Enqueue(gn);

                while (queue.Count > 0) {
                    // process the current vertex
                    var vertex = queue.Dequeue();
                    if (vertex.edge != null) {
                        spikeNodes.Add(vertex);
                    }

                    // get all adjacent, and enqueue any unvisited
                    foreach (var adjacent in vertex.links) {
                        if (!visited.ContainsKey(adjacent) || !visited[adjacent]) { // not visited
                            visited[adjacent] = true; // mark visited
                            queue.Enqueue(adjacent); // enqueue
                        }
                    }
                }

                // then, we will x2 loop through all spike nodes and match spike to spike via link equiv
                foreach (var spk1 in spikeNodes) {
                    foreach (var spk2 in spikeNodes) {
                        if (spk1 == spk2) continue;
                        // now, match spike-to-spike
                        if (spk1.edge.equiv(spk2.edge)) {
                            // we have a match!
                            spk1.links.Add(spk2);
                            spk2.links.Add(spk1);
                        }
                    }
                }

                // this should give us a graph!
            }
        }
    }
}