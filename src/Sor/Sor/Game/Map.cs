using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Game {
    /// <summary>
    /// a data structure representing a model of the map
    /// </summary>
    public class Map {
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

            public List<Door> doors = new List<Door>();

            public Room(Point ul, Point dr) {
                this.ul = ul;
                this.dr = dr;
                this.center = new Point((ul.X + dr.X) / 2, (ul.Y + dr.Y) / 2);
            }
        }

        public class Door {
            public Point start;
            public Point end;
            public Direction dir;

            public Door(Point start, Point end, Direction dir) {
                this.start = start;
                this.end = end;
                this.dir = dir;
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