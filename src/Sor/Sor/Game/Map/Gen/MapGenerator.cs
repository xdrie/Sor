using System.Collections.Generic;
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

        public void copyToTilemap(TmxMap map) {
            var structure = map.GetLayer<TmxLayer>(MapLoader.LAYER_STRUCTURE);
            // reset size to be big
            resizeTmxLayer(structure, 2000, 2000);
            // set all the tiles to be big
            for (int sy = 0; sy < structure.Height; sy++) {
                for (int sx = 0; sx < structure.Width; sx++) {
                    // var tile = structure.GetTile(sx, sy);
                    // structure.SetTile(sx, sy, )
                    // structure.SetTile(new TmxLayerTile(map, 3, sx, sy));
                }
            }
        }
    }
}