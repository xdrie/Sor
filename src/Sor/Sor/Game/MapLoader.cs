using System;
using System.Collections.Generic;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Sor.Components.Things;

namespace Sor.Game {
    public class MapLoader {
        private Scene scene;
        private readonly Entity mapEntity;
        private TmxLayer structure;
        private TmxLayer features;
        private TmxObjectGroup nature;
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
            features = map.GetLayer<TmxLayer>("features");
            nature = map.GetObjectGroup("nature");
            worldTileset = map.Tilesets["world_tiles"];
            adjustColliders();
            analyzeRooms();
            loadFeatures();
            loadNature();
            Global.log.writeLine("loaded data from map", GlintLogger.LogLevel.Information);
        }

        private void loadNature() {
            foreach (var th in nature.Objects) {
                if (th.Type == "tree") {
                    var nt = scene.CreateEntity(th.Name, new Vector2(th.X, th.Y));
                    var treeStage = 1;
                    if (th.Properties.TryGetValue("stage", out var stageProp)) {
                        treeStage = int.Parse(stageProp);
                    }
                    nt.AddComponent(new Tree {stage = treeStage});
                    Global.log.writeLine($"tree L{treeStage}: ({nt.Name}, {nt.Position})", GlintLogger.LogLevel.Trace);
                }
            }
        }

        private void loadFeatures() {
            // look for colliders (they will be matched to tile groups)
            // different types of features are never adjacent, so checking ul tile will tell us the kind
            var rects = features.GetCollisionRectangles();
            foreach (var collider in rects) {
                var boxCollider = new BoxCollider(collider);
                boxCollider.IsTrigger = true;
                boxCollider.Tag = Constants.COLLIDER_LANE;
                mapEntity.AddComponent(boxCollider);
            }
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
                        return analyzeWallTile((Map.TileKind) (t.Gid - worldTileset.FirstGid), tileDirection(t));
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
                            }
                            else {
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
                        Global.log.writeLine($"room ul:{room.ul}, dr{room.dr}, doors:{room.doors.Count})", GlintLogger.LogLevel.Trace);
                    }
                }
            }
        }

        /// <summary>
        /// Re-calculate the colliders to better match the tile sprites
        /// </summary>
        private void adjustColliders() {
            var rects = structure.GetCollisionRectangles();
            var adjustedColliders = new List<Rectangle>();
            foreach (var rect in rects) {
                // TODO: since rect center is checked, it might be an opening and then the wall won't be detected
                var rectCenter = new Vector2(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                var rectLeft = new Vector2(rect.Left, rect.Top + rect.Height / 2);
                var corrTilePos = map.WorldToTilePosition(rectCenter);
                var corrTile = structure.GetTile(corrTilePos.X, corrTilePos.Y);
                if (corrTile == null) { // if center isn't found, check left
                    corrTilePos = map.WorldToTilePosition(rectLeft);
                    corrTile = structure.GetTile(corrTilePos.X, corrTilePos.Y);
                }
                var corrDirection = tileDirection(corrTile);
                var adjCollider = default(Rectangle);
                // adjust collider based on direction
                switch (corrDirection) {
                    case Direction.Up: // left wall
                        adjCollider = new Rectangle(rect.X, rect.Y - map.TileWidth + WALL_BORDER, WALL_BORDER,
                            rect.Height + map.TileWidth - WALL_BORDER);
                        break;
                    case Direction.Right: // top wall
                        adjCollider = new Rectangle(rect.X, rect.Y, rect.Width, WALL_BORDER);
                        break;
                    case Direction.Down: // right wall
                        adjCollider = new Rectangle(rect.X + map.TileWidth - WALL_BORDER,
                            rect.Y - map.TileWidth + WALL_BORDER, WALL_BORDER,
                            rect.Height + map.TileWidth - WALL_BORDER);
                        break;
                    case Direction.Left: // down wall
                        adjCollider = new Rectangle(rect.X - map.TileWidth + WALL_BORDER,
                            rect.Y + map.TileWidth - WALL_BORDER,
                            rect.Width + map.TileWidth - WALL_BORDER + map.TileWidth - WALL_BORDER, WALL_BORDER);
                        break;
                }

                adjustedColliders.Add(adjCollider);
            }

            // add colliders to map
            foreach (var coll in adjustedColliders) {
                var boxCollider = new BoxCollider(coll);
                mapEntity.AddComponent(boxCollider);
                boxCollider.Tag = Constants.COLLIDER_WALL;
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

        private Map.TileOri analyzeWallTile(Map.TileKind tk, Direction dir) {
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