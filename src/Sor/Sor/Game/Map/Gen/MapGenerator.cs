using System.Collections.Generic;
using System.Linq;
using System.Text;
using LunchLib.Calc;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;

namespace Sor.Game.Map.Gen {
    public class MapGenerator {
        public int width;
        public int height;
        public int[] grid;
        private DiscreteProbabilityDistribution<int> roomWall;
        public List<Rectangle> roomRects = new List<Rectangle>();
        private Dictionary<Rectangle, Map.Room> rectToRooms;
        private RoomGraph graph;
        private const int cellTileSize = 8;
        private const int cellDoorSize = 5;
        private const int cellTilePadding = 6;

        public MapGenerator(int width, int height) {
            this.width = width;
            this.height = height;
            grid = new int[width * height];

            roomWall = new DiscreteProbabilityDistribution<int>(new[] {
                (0.2f, 0),
                (0.2f, 1),
                (0.35f, 2),
                (0.2f, 3),
                (0.05f, 4)
            });
        }

        private bool overlapsAnyRect(Rectangle rect) {
            foreach (var roomRect in roomRects) {
                if (roomRect.Intersects(rect)) return true;
            }

            return false;
        }

        private void addRoomRect(Rectangle newRoomRect) {
            roomRects.Add(newRoomRect);
            // set cells in the grid
            for (int r = newRoomRect.X; r < newRoomRect.X + newRoomRect.Width; r++) {
                for (int c = newRoomRect.Y; c < newRoomRect.Y + newRoomRect.Height; c++) {
                    grid[r * width + c] = roomRects.Count;
                }
            }
        }

        public void generate() {
            for (int sy = 0; sy < height; sy++) {
                for (int sx = 0; sx < width; sx++) {
                    var roomW = roomWall.next();
                    var roomH = roomWall.next();
                    var roomSz = roomW * roomH;
                    // a size of 0w or 0h means skip this room
                    if (roomSz == 0) {
                        continue;
                    }

                    // check validity
                    var newRoomRect = new Rectangle(sx, sy, roomW, roomH);
                    if (sy + roomH > height || sx + roomW > width) { // ensure bounds
                        continue;
                    }

                    if (overlapsAnyRect(newRoomRect)) {
                        // overlap, skip
                        continue;
                    }

                    // add to room rects
                    addRoomRect(newRoomRect);
                    // set values in the grid
                }
            }
        }

        private static void resizeTmxLayer(TmxLayer layer, int width, int height) {
            var oldWidth = layer.Width;
            var oldHeight = layer.Height;
            layer.Width = width;
            layer.Height = height;
            var index = 0;
            // copy old gids
            var oldTiles = (TmxLayerTile[]) layer.Tiles.Clone();
            // and create new tile buffer
            var newTileBufSize = width * height;
            layer.Tiles = new TmxLayerTile[newTileBufSize];
            for (var j = 0; j < oldHeight; j++)
            for (var i = 0; i < oldWidth; i++) {
                if (index >= newTileBufSize) break;
                var oldTile = oldTiles[index];
                var gid = default(int);
                if (oldTile != null) {
                    gid = oldTile.Gid;
                    layer.Tiles[index] = new TmxLayerTile(layer.Map, (uint) gid, i, j);
                } else {
                    layer.Tiles[index] = null;
                }

                index++;
            }
        }

        public void analyze() {
            rectToRooms = new Dictionary<Rectangle, Map.Room>();
            // map rects to rooms
            foreach (var rect in roomRects) {
                rectToRooms[rect] = new Map.Room(new Point(rect.Left, rect.Top),
                    new Point(rect.Right, rect.Bottom));
            }

            // analyze grid and build graph
            for (int c = 0; c < width; c++) {
                for (int r = 0; r < height; r++) {
                    var cpt = new Point(r, c);
                    // find rect containing
                    var currRect = default(Rectangle);
                    bool foundRect = false;
                    foreach (var rect in roomRects) {
                        if (rect.Contains(cpt)) {
                            currRect = rect;
                            foundRect = true;
                            break;
                        }
                    }

                    if (!foundRect) continue;

                    var cell = grid[r * width + c];
                    // check neighbors
                    for (var dx = -1; dx <= 1; dx++) {
                        for (var dy = -1; dy <= 1; dy++) {
                            var sc = c + dx;
                            var sr = r + dy;
                            // check bounds
                            if (sc < 0 || sr < 0 || sc >= width || sr >= height) continue;
                            var spt = new Point(sr, sc);
                            if (currRect.Contains(spt)) continue;
                            var neigh = grid[sr * width + sc];
                            if (neigh > 0 && cell != neigh) {
                                // this is a different room, figure out which one and add a link
                                var neighRect = roomRects.Single(x => x.Contains(spt));
                                var otherRoom = rectToRooms[neighRect];
                                var currRoom = rectToRooms[currRect];
                                currRoom.links.Add(otherRoom);
                            }
                        }
                    }
                }
            }

            graph = new RoomGraph(rectToRooms.Values.ToList());
        }

        private static TmxLayerTile pickTile(TmxMap map, int x, int y, Map.TileKind kind, Map.TileOri ori) {
            var tid = 0U;
            switch (kind) {
                case Map.TileKind.Corner:
                    tid = 3;
                    break;
                case Map.TileKind.Wall:
                    tid = 2;
                    break;
            }

            var tile = new TmxLayerTile(map, tid, x, y);

            var tileDir = default(Direction);
            switch (ori) {
                case Map.TileOri.Left:
                    tileDir = Direction.Up;
                    break;
                case Map.TileOri.Up:
                    tileDir = Direction.Right;
                    break;
                case Map.TileOri.Right:
                    tileDir = Direction.Down;
                    break;
                case Map.TileOri.Down:
                    tileDir = Direction.Left;
                    break;
                case Map.TileOri.UpLeft:
                    tileDir = Direction.Right;
                    break;
                case Map.TileOri.UpRight:
                    tileDir = Direction.Down;
                    break;
                case Map.TileOri.DownRight:
                    tileDir = Direction.Left;
                    break;
                case Map.TileOri.DownLeft:
                    tileDir = Direction.Up;
                    break;
            }

            // now, map tile dir to flips
            switch (tileDir) {
                case Direction.Up:
                    tile.DiagonalFlip = false;
                    tile.HorizontalFlip = false;
                    tile.VerticalFlip = false;
                    break;
                case Direction.Right:
                    tile.DiagonalFlip = true;
                    tile.HorizontalFlip = true;
                    tile.VerticalFlip = false;
                    break;
                case Direction.Down:
                    tile.DiagonalFlip = false;
                    tile.HorizontalFlip = true;
                    tile.VerticalFlip = true;
                    break;
                case Direction.Left:
                    tile.DiagonalFlip = true;
                    tile.HorizontalFlip = false;
                    tile.VerticalFlip = true;
                    break;
            }

            return tile;
        }

        private void createRoom(TmxLayer structure, Map.Room room) {
            // figure out UL point
            int scaleCell(int c) {
                return c * (cellTileSize + cellTilePadding);
            }

            // put in the corners
            var ulx = scaleCell(room.x);
            var uly = scaleCell(room.y);
            var brx = ulx + scaleCell(room.width) - cellTilePadding;
            var bry = uly + scaleCell(room.height) - cellTilePadding;
            var ulCorner = pickTile(structure.Map, ulx, uly, Map.TileKind.Corner, Map.TileOri.UpLeft);
            structure.SetTile(ulCorner);
            var brCorner = pickTile(structure.Map, brx, bry, Map.TileKind.Corner, Map.TileOri.DownRight);
            structure.SetTile(brCorner);
            var urx = brx;
            var ury = uly;
            var urCorner = pickTile(structure.Map, urx, ury, Map.TileKind.Corner, Map.TileOri.UpRight);
            structure.SetTile(urCorner);
            var blx = ulx;
            var bly = bry;
            var blCorner = pickTile(structure.Map, blx, bly, Map.TileKind.Corner, Map.TileOri.DownLeft);
            structure.SetTile(blCorner);

            // set walls
            for (int sx = ulx + 1; sx < brx; sx++) { // upper wall
                structure.SetTile(pickTile(structure.Map, sx, uly, Map.TileKind.Wall, Map.TileOri.Up));
            }

            for (int sy = uly + 1; sy < bry; sy++) { // right wall
                structure.SetTile(pickTile(structure.Map, brx, sy, Map.TileKind.Wall, Map.TileOri.Right));
            }

            for (int sx = brx - 1; sx > ulx; sx--) { // lower wall
                structure.SetTile(pickTile(structure.Map, sx, bry, Map.TileKind.Wall, Map.TileOri.Down));
            }

            for (int sy = bry - 1; sy > uly; sy--) { // left wall
                structure.SetTile(pickTile(structure.Map, ulx, sy, Map.TileKind.Wall, Map.TileOri.Left));
            }

            // carve doors for all links
            foreach (var link in room.links) {
                var linkDirection = default(Direction);
                if (link.center.X > room.center.X) { // right link
                    linkDirection = Direction.Right;
                }

                if (link.center.Y > room.center.Y) { // down link
                    linkDirection = Direction.Down;
                }

                if (link.center.X < room.center.X) { // left link
                    linkDirection = Direction.Left;
                }

                if (link.center.Y < room.center.Y) { // up link
                    linkDirection = Direction.Up;
                }

                // based on the direction, carve a door
                var csx = 0; // carve start x
                var csy = 0; // carve start y
                var dx = 0; // carve dx
                var dy = 0; // carve dy
                switch (linkDirection) {
                    case Direction.Up: // cut in center of upper wall
                        csx = (ulx + brx) / 2 - cellDoorSize / 2;
                        csy = uly;
                        dx = 1;
                        break;
                    case Direction.Right:
                        csx = brx;
                        csy = (uly + bry) / 2 - cellDoorSize / 2;
                        dy = 1;
                        break;
                    case Direction.Down:
                        csx = (ulx + brx) / 2 - cellDoorSize / 2;
                        csy = bry;
                        dx = 1;
                        break;
                    case Direction.Left:
                        csx = ulx;
                        csy = (uly + bry) / 2 - cellDoorSize / 2;
                        dy = 1;
                        break;
                }

                // carve
                var laserX = csx;
                var laserY = csy;
                for (var crx = 0; crx < cellDoorSize; crx += 1) {
                    for (var cry = 0; cry < cellDoorSize; cry += 1) {
                        // update laser pos
                        laserX = csx + crx * dx;
                        laserY = csy + cry * dy;
                        // unset the tile
                        structure.RemoveTile(laserX, laserY);
                    }
                }
            }
        }

        public void copyToTilemap(TmxMap map) {
            var structure = map.GetLayer<TmxLayer>(MapLoader.LAYER_STRUCTURE);
            // // reset size to be big
            // resizeTmxLayer(structure, 2000, 2000);
            // // set all the tiles to be big
            // for (int sy = 0; sy < structure.Height; sy++) {
            //     for (int sx = 0; sx < structure.Width; sx++) {
            //         // var tile = structure.GetTile(sx, sy);
            //         structure.SetTile(new TmxLayerTile(map, 3, sx, sy));
            //     }
            // }

            // create rooms
            foreach (var room in graph.rooms) {
                // render the room to tiles
                createRoom(structure, room);
            }

            // // attempt to test picktile
            // structure.SetTile(pickTile(map, 0, 0, Map.TileKind.Corner, Map.TileOri.UpLeft));
            // structure.SetTile(pickTile(map, 1, 0, Map.TileKind.Wall, Map.TileOri.Up));
            // structure.SetTile(pickTile(map, 2, 0, Map.TileKind.Wall, Map.TileOri.Up));
            // structure.SetTile(pickTile(map, 3, 0, Map.TileKind.Wall, Map.TileOri.Up));
            // structure.SetTile(pickTile(map, 4, 0, Map.TileKind.Corner, Map.TileOri.UpRight));
            // structure.SetTile(pickTile(map, 4, 1, Map.TileKind.Wall, Map.TileOri.Right));
            // structure.SetTile(pickTile(map, 4, 2, Map.TileKind.Wall, Map.TileOri.Right));
            // structure.SetTile(pickTile(map, 4, 3, Map.TileKind.Wall, Map.TileOri.Right));
            // structure.SetTile(pickTile(map, 4, 4, Map.TileKind.Corner, Map.TileOri.DownRight));
            // structure.SetTile(pickTile(map, 3, 4, Map.TileKind.Wall, Map.TileOri.Down));
            // structure.SetTile(pickTile(map, 2, 4, Map.TileKind.Wall, Map.TileOri.Down));
            // structure.SetTile(pickTile(map, 1, 4, Map.TileKind.Wall, Map.TileOri.Down));
            // structure.SetTile(pickTile(map, 0, 4, Map.TileKind.Corner, Map.TileOri.DownLeft));
            // structure.SetTile(pickTile(map, 0, 3, Map.TileKind.Wall, Map.TileOri.Left));
            // structure.SetTile(pickTile(map, 0, 2, Map.TileKind.Wall, Map.TileOri.Left));
            // structure.SetTile(pickTile(map, 0, 1, Map.TileKind.Wall, Map.TileOri.Left));
        }

        public string dumpGrid() {
            var sb = new StringBuilder();
            for (int c = 0; c < width; c++) {
                for (int r = 0; r < height; r++) {
                    var cell = grid[r * width + c];
                    sb.Append($"{cell,4}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}