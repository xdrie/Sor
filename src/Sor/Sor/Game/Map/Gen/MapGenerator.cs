using System.Collections.Generic;
using System.Linq;
using LunchLib.Calc;
using Microsoft.Xna.Framework;
using Nez.Tiled;

namespace Sor.Game.Map.Gen {
    public class MapGenerator {
        public int width;
        public int height;
        public int[] grid;
        private DiscreteProbabilityDistribution<int> roomWall;
        private List<Rectangle> roomRects = new List<Rectangle>();
        private Dictionary<Rectangle, Map.Room> rectToRooms;
        private RoomGraph graph;

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

        private void createRoom(TmxMap map, Map.Room room) {
            // figure out UL point
            room.ul
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
        }
    }
}