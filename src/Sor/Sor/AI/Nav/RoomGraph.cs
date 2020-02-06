using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.Pathfinding;
using Sor.Game;

namespace Sor.AI.Nav {
    public class RoomGraph : IWeightedGraph<Map.Room> {
        public List<Map.Room> rooms;
        
        public IEnumerable<Map.Room> GetNeighbors(Map.Room node) {
            return node.links;
        }

        public int Cost(Map.Room @from, Map.Room to) {
            // TODO: figure out a better way to do this
            // for now, base it on room center proximity
            var dist = PointExt.mhDist(from.center, to.center);
            return dist;
        }
    }
}