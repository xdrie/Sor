using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glint;
using Glint.Util;
using LunchLib.Calc;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;

namespace Sor.Game.Map.Gen {
    public class MapGenerator {
        private Dictionary<Rectangle, Map.Room> rectToRooms;
        private RoomGraph graph;
        private Rng rng;

        private DiscreteProbabilityDistribution<int> roomWallDistr;
        private DiscreteProbabilityDistribution<int> treeLevelDistr;

        private const int CELL_TILE_SIZE = 12; // the size in tiles that a cell converts to
        private const int CELL_DOOR_SIZE = 5; // size of the doors cut into rooms
        private const int CELL_TILE_SPACING = 10; // spacing between map grid cells
        private const int ROOM_OBJECT_PADDING = 2; // spacing between the room walls and objects 

        public int width;
        public int height;
        public int[] grid;
        public List<Rectangle> roomRects = new List<Rectangle>();

        public MapGenerator(int width, int height, int seed) {
            this.width = width;
            this.height = height;
            grid = new int[width * height];

            rng = new Rng(seed);
            roomWallDistr = new DiscreteProbabilityDistribution<int>(rng, new[] {
                (0.2f, 0),
                (0.2f, 1),
                (0.35f, 2),
                (0.2f, 3),
                (0.05f, 4)
            });
            treeLevelDistr = new DiscreteProbabilityDistribution<int>(rng, new[] {
                (0.2f, 1),
                (0.2f, 2),
                (0.2f, 3),
                (0.1f, 4),
                (0.1f, 5),
                (0.06f, 6),
                (0.04f, 7),
                (0.03f, 8),
                (0.02f, 9),
            });
        }

        public void setGrid(int x, int y, int v) {
            grid[y * width + x] = v;
        }

        public int getGrid(int x, int y) {
            return grid[y * width + x];
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
            for (var sx = newRoomRect.X; sx < newRoomRect.X + newRoomRect.Width; sx++) {
                for (var sy = newRoomRect.Y; sy < newRoomRect.Y + newRoomRect.Height; sy++) {
                    setGrid(sx, sy, roomRects.Count);
                }
            }
        }

        public void generate() {
            for (var sy = 0; sy < height; sy++) {
                for (var sx = 0; sx < width; sx++) {
                    var roomW = roomWallDistr.next();
                    var roomH = roomWallDistr.next();
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
                rectToRooms[rect] =
                    new Map.Room(new Point(rect.Left, rect.Top),
                        new Point(rect.Right, rect.Bottom));
                // these are mini/fake rooms on the cell grid scale, not on the tile scale
            }

            // analyze grid and build graph
            for (var asy = 0; asy < width; asy++) {
                for (var asx = 0; asx < height; asx++) {
                    var cpt = new Point(asx, asy);
                    // find rect containing
                    var currRect = default(Rectangle);
                    var foundRect = false;
                    foreach (var rect in roomRects) {
                        if (rect.Contains(cpt)) {
                            currRect = rect;
                            foundRect = true;
                            break;
                        }
                    }

                    if (!foundRect) continue;

                    var cell = getGrid(asx, asy);
                    // check neighbors
                    for (var dx = -1; dx <= 1; dx++) {
                        for (var dy = -1; dy <= 1; dy++) {
                            // scan position of neighbor
                            var neighY = asy + dx;
                            var neighX = asx + dy;
                            // check bounds
                            if (neighY < 0 || neighX < 0 || neighY >= width || neighX >= height) continue;
                            var spt = new Point(neighX, neighY);
                            if (currRect.Contains(spt)) continue;
                            var neigh = getGrid(neighX, neighY);
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

        private int cellToTilePos(int c) {
            return c * (CELL_TILE_SIZE + CELL_TILE_SPACING);
        }

        private void createRoom(TmxLayer structure, Map.Room room) {
            // figure out UL point

            // put in the corners
            var ulp = new Point(cellToTilePos(room.x), cellToTilePos(room.y));
            var brp = new Point(ulp.X + cellToTilePos(room.width) - CELL_TILE_SPACING,
                ulp.Y + cellToTilePos(room.height) - CELL_TILE_SPACING);
            var urp = new Point(brp.X, ulp.Y);
            var blp = new Point(ulp.X, brp.Y);

            var ulCorner = pickTile(structure.Map, ulp.X, ulp.Y, Map.TileKind.Corner, Map.TileOri.UpLeft);
            structure.SetTile(ulCorner);
            var brCorner = pickTile(structure.Map, brp.X, brp.Y, Map.TileKind.Corner, Map.TileOri.DownRight);
            structure.SetTile(brCorner);

            var urCorner = pickTile(structure.Map, urp.X, urp.Y, Map.TileKind.Corner, Map.TileOri.UpRight);
            structure.SetTile(urCorner);
            var blCorner = pickTile(structure.Map, blp.X, blp.Y, Map.TileKind.Corner, Map.TileOri.DownLeft);
            structure.SetTile(blCorner);

            Global.log.writeLine($"room tilegen from ul:{ulp}, ur:{urp}, br:{brp}, bl:{blp}",
                GlintLogger.LogLevel.Trace);

            // set walls
            for (var sx = ulp.X + 1; sx < brp.X; sx++) { // upper wall
                structure.SetTile(pickTile(structure.Map, sx, ulp.Y, Map.TileKind.Wall, Map.TileOri.Up));
            }

            for (var sy = ulp.Y + 1; sy < brp.Y; sy++) { // right wall
                structure.SetTile(pickTile(structure.Map, brp.X, sy, Map.TileKind.Wall, Map.TileOri.Right));
            }

            for (var sx = brp.X - 1; sx > ulp.X; sx--) { // lower wall
                structure.SetTile(pickTile(structure.Map, sx, brp.Y, Map.TileKind.Wall, Map.TileOri.Down));
            }

            for (var sy = brp.Y - 1; sy > ulp.Y; sy--) { // left wall
                structure.SetTile(pickTile(structure.Map, ulp.X, sy, Map.TileKind.Wall, Map.TileOri.Left));
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
                        csx = (ulp.X + brp.X) / 2 - CELL_DOOR_SIZE / 2;
                        csy = ulp.Y;
                        dx = 1;
                        break;
                    case Direction.Right:
                        csx = brp.X;
                        csy = (ulp.Y + brp.Y) / 2 - CELL_DOOR_SIZE / 2;
                        dy = 1;
                        break;
                    case Direction.Down:
                        csx = (ulp.X + brp.X) / 2 - CELL_DOOR_SIZE / 2;
                        csy = brp.Y;
                        dx = 1;
                        break;
                    case Direction.Left:
                        csx = ulp.X;
                        csy = (ulp.Y + brp.Y) / 2 - CELL_DOOR_SIZE / 2;
                        dy = 1;
                        break;
                }

                // carve
                var laserX = csx;
                var laserY = csy;
                for (var crx = 0; crx < CELL_DOOR_SIZE; crx += 1) {
                    for (var cry = 0; cry < CELL_DOOR_SIZE; cry += 1) {
                        // update laser pos
                        laserX = csx + crx * dx;
                        laserY = csy + cry * dy;
                        // unset the tile
                        structure.RemoveTile(laserX, laserY);
                    }
                }
            }
        }

        private void placeTree(TmxObjectGroup nature, Map.Room room, int stage) {
            // 1. pick a random spot in the room
            var placePoint = new Point(
                rng.next(cellToTilePos(room.x) + ROOM_OBJECT_PADDING,
                    cellToTilePos(room.x + room.width) - CELL_TILE_SPACING - ROOM_OBJECT_PADDING),
                rng.next(cellToTilePos(room.y) + ROOM_OBJECT_PADDING,
                    cellToTilePos(room.y + room.height) - CELL_TILE_SPACING - ROOM_OBJECT_PADDING));
            // var placePoint = new Point(cellToTilePos(room.x), cellToTilePos(room.y));
            // 2. ensure the spot is empty
            foreach (var obj in nature.Objects) {
                if (obj.Type == MapLoader.OBJECT_TREE) {
                    var objX = (int) obj.X;
                    var objY = (int) obj.Y;
                    if (objX == placePoint.X && objY == placePoint.Y) {
                        // the pieces overlap, fail
                        return;
                    }
                }
            }

            // 3. add a tree object
            nature.Objects.Add(new TmxObject {
                X = nature.Map.TileToWorldPositionX(placePoint.X),
                Y = nature.Map.TileToWorldPositionY(placePoint.Y),
                Name = $"{MapLoader.OBJECT_TREE}_{placePoint.X}_{placePoint.Y}",
                Type = MapLoader.OBJECT_TREE,
                Properties = new Dictionary<string, string> {
                    {MapLoader.OBJECT_PROP_STAGE, stage.ToString()}
                },
                ObjectType = TmxObjectType.Basic,
                Width = nature.Map.TileWidth,
                Height = nature.Map.TileHeight
            });
        }

        public void copyToTilemap(TmxMap map, bool createObjects) {
            var structure = map.GetLayer<TmxLayer>(MapLoader.LAYER_STRUCTURE);
            var nature = map.GetObjectGroup(MapLoader.LAYER_NATURE);
            // clear/reset the map
            // 1. clear the structure map
            var srWidth = Math.Max((width + 1) * (CELL_TILE_SIZE + CELL_TILE_SPACING), map.Width);
            var srHeight = Math.Max((height + 1) * (CELL_TILE_SIZE + CELL_TILE_SPACING), map.Height);
            resizeTmxLayer(structure, srWidth, srHeight);
            // clear all the tiles
            for (int sy = 0; sy < structure.Height; sy++) {
                for (int sx = 0; sx < structure.Width; sx++) {
                    structure.RemoveTile(sx, sy);
                }
            }

            // 2. clear the nature map
            nature.Objects.Clear();

            // create rooms
            foreach (var room in graph.rooms) {
                // render the room to tiles
                createRoom(structure, room);
            }

            if (createObjects) {
                // create objects
                // TODO: improve this object generation
                // for now, put a random-leveled tree in each room
                foreach (var room in graph.rooms) {
                    var treeLevel = treeLevelDistr.next();
                    placeTree(nature, room, treeLevel);
                }
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
            for (var sy = 0; sy < height; sy++) {
                for (var sx = 0; sx < width; sx++) {
                    var cell = getGrid(sx, sy);
                    sb.Append($"{cell,4}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}