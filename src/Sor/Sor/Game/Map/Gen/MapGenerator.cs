using System.Collections.Generic;
using LunchLib.Calc;
using Microsoft.Xna.Framework;

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
                (0.4f, 1),
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
            for (int r = newRoomRect.X; r <= newRoomRect.X + newRoomRect.Width; r++) {
                for (int c = newRoomRect.Y; c <= newRoomRect.Y + newRoomRect.Height; c++) {
                    grid[r * width + c] = roomRects.Count;
                }
            }
        }

        public void generate() {
            for (int sy = 0; sy < height; sy++) {
                for (int sx = 0; sx < width; sx++) {
                    var roomW = roomWall.next(); // wall of size 1 means a rect of W 0, because 1x1 is a single cell
                    var roomH = roomWall.next();
                    // check validity
                    var newRoomRect = new Rectangle(sx, sy, roomW - 1, roomH - 1);
                    if (sy + roomH > height - 1 || sx + roomW > width - 1) { // ensure bounds
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
    }
}