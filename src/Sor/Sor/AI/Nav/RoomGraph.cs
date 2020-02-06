using System.Collections.Generic;
using Nez.AI.Pathfinding;
using Sor.Game;

namespace Sor.AI.Nav {
    public class RoomGraph : IWeightedGraph<Map.Room> {
        public IEnumerable<Map.Room> GetNeighbors(Map.Room node) {
            throw new System.NotImplementedException();
        }

        public int Cost(Map.Room @from, Map.Room to) {
            throw new System.NotImplementedException();
        }
    }
}