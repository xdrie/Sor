using Nez.Tiled;

namespace Sor.Game.Map {
    /// <summary>
    /// an inspectable, analyzed representation of the map
    /// </summary>
    public class MapRepr {
        /// <summary>
        /// the raw map tiles
        /// </summary>
        public TmxMap tmxMap;
        /// <summary>
        /// navigation graph data structure of the map
        /// </summary>
        public StructuralNavigationGraph sng;
        /// <summary>
        /// navigation graph of the rooms
        /// </summary>
        public RoomGraph roomGraph;
    }
}