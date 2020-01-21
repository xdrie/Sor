using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Game {
    /// <summary>
    /// a data structure representing a model of the map
    /// </summary>
    public class MindMap {
        public class Room {
            /// <summary>
            /// ul-left corner
            /// </summary>
            public Point ul;
            
            /// <summary>
            /// down-right corner
            /// </summary>
            public Point dr;

            public Point center;

            public Room(Point ul, Point dr) {
                this.ul = ul;
                this.dr = dr;
                this.center = new Point((ul.X + dr.X) / 2, (ul.Y + dr.Y) / 2);
            }
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