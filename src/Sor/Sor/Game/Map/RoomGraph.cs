using System.Collections.Generic;
using Nez;
using Nez.AI.Pathfinding;

namespace Sor.Game.Map {
    public class RoomGraph : IAstarGraph<Map.Room> {
        public readonly List<Map.Room> rooms;

        public RoomGraph(List<Map.Room> rooms) {
            this.rooms = rooms;
        }

        public IEnumerable<Map.Room> GetNeighbors(Map.Room node) {
            return node.links;
        }

        public int Cost(Map.Room @from, Map.Room to) {
            // TODO: figure out a better way to do this
            // for now, base it on room center proximity
            var dist = PointExt.mhDist(from.center, to.center);
            return dist;
        }

        public int Heuristic(Map.Room node, Map.Room goal) {
            return 0;
        }
    }
}