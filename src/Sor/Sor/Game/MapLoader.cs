using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;

namespace Sor.Game {
    public class MapLoader {
        private Scene scene;
        private readonly Entity mapEntity;
        private TmxLayer structure;
        private TmxTileset worldTileset;
        private TmxMap map;
        public const int WALL_BORDER = 4;

        public MapLoader(Scene scene, Entity mapEntity) {
            this.scene = scene;
            this.mapEntity = mapEntity;
        }

        public void load(TmxMap map) {
            this.map = map;
            structure = map.GetLayer<TmxLayer>("structure");
            worldTileset = map.Tilesets["world_tiles"];
            adjustColliders();
            analyzeRooms();
        }

        /// <summary>
        /// Convert the tilemap into a better data structure
        /// </summary>
        private void analyzeRooms() {
            for (int r = 0; r < structure.Height; r++) {
                for (int c = 0; c < structure.Width; c++) {
                    var tile = structure.GetTile(c, r);
                    if (tile == null) continue;

                    Map.TileOri ori(TmxLayerTile t) {
                        return analyzeTile((Map.TileKind) (t.Gid - worldTileset.FirstGid), tileDirection(t));
                    }

                    var tileOri = ori(tile);
                    if (tileOri == Map.TileOri.UpLeft) {
                        var topEdge = r;
                        var leftEdge = c;
                        var scanFirst = default(Point);
                        var scanOpen = 0;
                        var openings = new List<Map.Door>();
                        // line-scan setup
                        void updateScan(TmxLayerTile t, Point p) {
                            if (t != null) {
                                if (scanOpen > 0) {
                                    openings.Add(new Map.Door(scanFirst, p));
                                    scanFirst = default;
                                    scanOpen = 0;
                                }
                            } else {
                                if (scanOpen == 0) {
                                    // start the count
                                    scanFirst = p;
                                }

                                scanOpen++;
                            }
                        }
                        // scan for an UpRight
                        var ulTile = tile;
                        var urTile = default(TmxLayerTile);
                        var rightEdge = -1;
                        for (int sx = leftEdge; sx < structure.Width; sx++) {
                            var scTile = structure.GetTile(sx, topEdge);
                            updateScan(scTile, new Point(sx, topEdge));
                            if (scTile == null) continue;
                            if (ori(scTile) == Map.TileOri.UpRight) {
                                urTile = scTile;
                                rightEdge = sx;
                                break;
                            }
                        }

                        if (urTile == null) break;
                        var drTile = default(TmxLayerTile);
                        var downEdge = -1;
                        for (int sy = topEdge; sy < structure.Height; sy++) {
                            var scTile = structure.GetTile(rightEdge, sy);
                            updateScan(scTile, new Point(rightEdge, sy));
                            if (scTile == null) continue;
                            if (ori(scTile) == Map.TileOri.DownRight) {
                                drTile = scTile;
                                downEdge = sy;
                                break;
                            }
                        }

                        if (drTile == null) break;
                        var dlTile = default(TmxLayerTile);
                        for (int sx = rightEdge; sx >= 0; sx--) {
                            var scTile = structure.GetTile(sx, downEdge);
                            updateScan(scTile, new Point(sx, downEdge));
                            if (scTile == null) continue;
                            if (ori(scTile) == Map.TileOri.DownLeft) {
                                dlTile = scTile;
                                break;
                            }
                        }

                        if (dlTile == null) break;
                        // all 4 corners have been found, create a room
                        var room = new Map.Room(new Point(leftEdge, topEdge), new Point(rightEdge, downEdge));
                        room.doors = openings;
                    }
                }
            }
        }

        /// <summary>
        /// Re-calculate the colliders to better match the tile sprites
        /// </summary>
        private void adjustColliders() {
            var colliders = structure.GetCollisionRectangles();
            var adjustedColliders = new List<Rectangle>();
            foreach (var collider in colliders) {
                var colliderCenter =
                    new Vector2(collider.Left + collider.Width / 2, collider.Top + collider.Height / 2);
                var corrTilePos = map.WorldToTilePosition(colliderCenter);
                var corrTile = structure.GetTile(corrTilePos.X, corrTilePos.Y);
                var corrDirection = tileDirection(corrTile);
                var adjCollider = default(Rectangle);
                // adjust collider based on direction
                switch (corrDirection) {
                    case Direction.Up: // left wall
                        adjCollider = new Rectangle(collider.X, collider.Y - map.TileWidth + WALL_BORDER, WALL_BORDER,
                            collider.Height + map.TileWidth - WALL_BORDER);
                        break;
                    case Direction.Right: // top wall
                        adjCollider = new Rectangle(collider.X, collider.Y, collider.Width, WALL_BORDER);
                        break;
                    case Direction.Down: // right wall
                        adjCollider = new Rectangle(collider.X + map.TileWidth - WALL_BORDER,
                            collider.Y - map.TileWidth + WALL_BORDER, WALL_BORDER,
                            collider.Height + map.TileWidth - WALL_BORDER);
                        break;
                    case Direction.Left: // down wall
                        adjCollider = new Rectangle(collider.X - map.TileWidth + WALL_BORDER,
                            collider.Y + map.TileWidth - WALL_BORDER,
                            collider.Width + map.TileWidth - WALL_BORDER + map.TileWidth - WALL_BORDER, WALL_BORDER);
                        break;
                }

                adjustedColliders.Add(adjCollider);
            }

            // add colliders to map
            foreach (var coll in adjustedColliders) {
                var boxCollider = new BoxCollider(coll);
                mapEntity.AddComponent(boxCollider);
                boxCollider.Tag = Constants.TAG_WALL_COLLIDER;
            }
        }

        private Direction tileDirection(TmxLayerTile tile) {
            var dir = Direction.Up;
            if (tile.DiagonalFlip && tile.HorizontalFlip && !tile.VerticalFlip)
                dir = Direction.Right;

            if (tile.DiagonalFlip && !tile.HorizontalFlip && tile.VerticalFlip)
                dir = Direction.Left;

            if (!tile.DiagonalFlip && tile.HorizontalFlip && tile.VerticalFlip)
                dir = Direction.Down;

            return dir;
        }

        private Map.TileOri analyzeTile(Map.TileKind tk, Direction dir) {
            if (tk == Map.TileKind.Wall) {
                switch (dir) {
                    case Direction.Up:
                        return Map.TileOri.Left;
                    case Direction.Right:
                        return Map.TileOri.Up;
                    case Direction.Down:
                        return Map.TileOri.Right;
                    case Direction.Left:
                        return Map.TileOri.Down;
                }
            }
            else if (tk == Map.TileKind.Corner) {
                switch (dir) {
                    case Direction.Up:
                        return Map.TileOri.DownLeft;
                    case Direction.Right:
                        return Map.TileOri.UpLeft;
                    case Direction.Down:
                        return Map.TileOri.UpRight;
                    case Direction.Left:
                        return Map.TileOri.DownRight;
                }
            }

            throw new FormatException("unrecognized tile");
        }
    }
}