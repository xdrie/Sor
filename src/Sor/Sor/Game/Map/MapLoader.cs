using System;
using System.Collections.Generic;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Sor.Components.Things;
using Sor.Util;

namespace Sor.Game.Map {
    public class MapLoader {
        private readonly PlayContext playContext;
        private readonly Entity mapEntity;
        private TmxLayer structure;
        private TmxLayer features;
        private TmxObjectGroup nature;
        private TmxTileset worldTileset;
        private TmxMap map;
        public MapRepr mapRepr;

        public const string LAYER_STRUCTURE = "structure";
        public const string LAYER_FEATURES = "features";
        public const string LAYER_BACKDROP = "backdrop";
        public const string LAYER_NATURE = "nature";
        public const string TILESET_WORLD = "world_tiles";
        public const string OBJECT_TREE = "tree";
        public const string OBJECT_PROP_STAGE = "stage";

        public const int WALL_BORDER = 4;
        public const int ROOM_LINK_DIST = 40;

        public MapLoader(PlayContext playContext, Entity mapEntity) {
            this.playContext = playContext;
            this.mapEntity = mapEntity;
        }

        public void load(TmxMap map, bool createObjects) {
            this.map = map;
            // structural recreation
            structure = map.GetLayer<TmxLayer>(LAYER_STRUCTURE);
            features = map.GetLayer<TmxLayer>(LAYER_FEATURES);
            nature = map.GetObjectGroup(LAYER_NATURE);
            worldTileset = map.Tilesets[TILESET_WORLD];
            if (NGame.context.config.enableWalls) createWallColliders(); // comment out to disable wall collision

            // analysis
            mapRepr = new MapRepr();
            mapRepr.tmxMap = map;
            analyzeRooms();
            mapRepr.sng = createStructuralNavigationGraph(mapRepr.roomGraph);

            // load entities
            loadFeatures();
            if (createObjects) {
                loadNature();
            }

            Global.log.writeLine("loaded data from map", Logger.Verbosity.Information);
        }

        private void loadNature() {
            foreach (var th in nature.Objects) {
                if (th.Type == OBJECT_TREE) {
                    var treeStage = 1; // default tree stage
                    if (th.Properties.TryGetValue(OBJECT_PROP_STAGE, out var stageProp)) {
                        treeStage = int.Parse(stageProp);
                    }

                    var nt = new Entity(th.Name)
                        .SetTag(Constants.Tags.THING);
                    nt.Position = new Vector2(th.X, th.Y);
                    var tree = nt.AddComponent(new Tree {stage = treeStage});
                    playContext.addThing(tree);
                    Global.log.writeLine($"tree L{treeStage}: ({nt.Name}, {nt.Position})", Logger.Verbosity.Trace);
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
                boxCollider.Tag = Constants.Colliders.LANE;
                mapEntity.AddComponent(boxCollider);
            }
        }

        /// <summary>
        /// Convert the tilemap into a better data structure
        /// </summary>
        private void analyzeRooms() {
            var rooms = new List<Map.Room>();
            // pass 1 - find all rooms
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
                        void updateScan(TmxLayerTile t, Point p, Direction dir) {
                            if (t != null) {
                                if (scanOpen > 0) {
                                    openings.Add(new Map.Door(scanFirst, p, dir));
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
                        for (int sx = leftEdge; sx < structure.Width; sx++) { // pass left-to-right along top
                            var scTile = structure.GetTile(sx, topEdge);
                            updateScan(scTile, new Point(sx, topEdge), Direction.Up);
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
                        for (int sy = topEdge; sy < structure.Height; sy++) { // pass top-to-bottom along right
                            var scTile = structure.GetTile(rightEdge, sy);
                            updateScan(scTile, new Point(rightEdge, sy), Direction.Right);
                            if (scTile == null) continue;
                            if (ori(scTile) == Map.TileOri.DownRight) {
                                drTile = scTile;
                                downEdge = sy;
                                break;
                            }
                        }

                        if (drTile == null) break;
                        var dlTile = default(TmxLayerTile);
                        for (int sx = rightEdge; sx >= 0; sx--) { // pass right-to left along down
                            var scTile = structure.GetTile(sx, downEdge);
                            updateScan(scTile, new Point(sx, downEdge), Direction.Down);
                            if (scTile == null) continue;
                            if (ori(scTile) == Map.TileOri.DownLeft) {
                                dlTile = scTile;
                                break;
                            }
                        }

                        if (dlTile == null) break;

                        // finally, check the left side
                        for (int sy = downEdge; sy >= 0; sy--) { // pass down-to-top along left
                            var scTile = structure.GetTile(leftEdge, sy);
                            updateScan(scTile, new Point(leftEdge, sy), Direction.Left);
                            if (scTile == null) continue;
                            if (ori(scTile) == Map.TileOri.UpLeft) { // we found her again
                                break;
                            }
                        }

                        // all 4 corners have been found, create a room
                        var room = new Map.Room(new Point(leftEdge, topEdge), new Point(rightEdge, downEdge));
                        room.doors = openings;
                        foreach (var door in openings) { // set local room of all doors
                            door.roomLocal = room;
                        }

                        rooms.Add(room);
                        Global.log.writeLine($"room ul:{room.ul}, dr{room.dr}, doors:{room.doors.Count})",
                            Logger.Verbosity.Trace);
                    }
                }
            }

            // pass 2 - determine room links
            foreach (var room in rooms) {
                foreach (var door in room.doors) {
                    // average the door pos
                    var inPos = door.doorCenter;
                    var (dx, dy) = DirectionStepper.stepIn(door.dir);

                    // now scan in direction
                    var distScanned = 0;
                    // set initial pos
                    var ix = inPos.X;
                    var iy = inPos.Y;
                    // set scan pos
                    var sx = ix;
                    var sy = iy;
                    while (distScanned < ROOM_LINK_DIST) {
                        // update scan vars
                        distScanned = Math.Abs(ix - sx) + Math.Abs(iy - sy);
                        sx += dx;
                        sy += dy;

                        // check if we're inside another room
                        var sPt = new Point(sx, sy);
                        // TODO: optimize this checking
                        // check if we're in any other room
                        var otherRoom = default(Map.Room);
                        foreach (var testRoom in rooms) {
                            if (testRoom.inRoom(sPt)) {
                                otherRoom = testRoom;
                                break;
                            }
                        }

                        if (otherRoom != null) {
                            // set up the connection
                            door.roomOther = otherRoom;
                            room.links.Add(otherRoom);
                            Global.log.writeLine(
                                $"room link [dist: {distScanned}] from Room[@{room.center}] to Room[@{otherRoom.center}]",
                                Logger.Verbosity.Trace);
                            break;
                        }
                    }
                }
            }

            // set up room graph
            mapRepr.roomGraph = new RoomGraph(rooms);
        }

        private StructuralNavigationGraph createStructuralNavigationGraph(RoomGraph rg) {
            var sngBuilder = new StructuralNavigationGraphBuilder(rg);
            sngBuilder.analyze();
            return sngBuilder.build();
        }

        /// <summary>
        /// Re-calculate the colliders to better match the tile sprites
        /// </summary>
        private void createWallColliders() {
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
                boxCollider.Tag = Constants.Colliders.WALL;
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
            } else if (tk == Map.TileKind.Corner) {
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