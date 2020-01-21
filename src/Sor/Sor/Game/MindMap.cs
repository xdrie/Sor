using Microsoft.Xna.Framework;

namespace Sor.Game {
    /// <summary>
    /// a data structure representing a model of the map
    /// </summary>
    public class MindMap {
        public class Room {
            /// <summary>
            /// top-left corner
            /// </summary>
            public Point tl;
            
            /// <summary>
            /// bottom-right corner
            /// </summary>
            public Point br;

            public Point center;
        }

        public enum TileKind {
            Backdrop = 0,
            Wall = 1,
            Corner = 2,
            Bip = 3,
        }

        public enum TileOri {
            UpLeft,
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left
        }
    }
}