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
                    var tile = structure.Tiles[r * structure.Width + c];
                    if (tile == null) continue;
                    var tileType = (MindMap.TileKind) (tile.Gid - worldTileset.FirstGid);
                    var tileDir = tileDirection(tile);
                    var tileOri = analyzeTile(tileType, tileDir);
                    // look for top-left corners
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
                var colliderCenter = new Vector2(collider.Left + collider.Width / 2, collider.Top + collider.Height / 2);
                var corrTilePos = map.WorldToTilePosition(colliderCenter);
                var corrTile = structure.GetTile(corrTilePos.X, corrTilePos.Y);
                var corrDirection = tileDirection(corrTile);
                var adjCollider = default(Rectangle);
                // adjust collider based on direction
                switch (corrDirection) {
                    case Direction.Up: // left wall
                        adjCollider = new Rectangle(collider.X, collider.Y - map.TileWidth + WALL_BORDER, WALL_BORDER, collider.Height + map.TileWidth - WALL_BORDER);
                        break;
                    case Direction.Right: // top wall
                        adjCollider = new Rectangle(collider.X, collider.Y, collider.Width, WALL_BORDER);
                        break;
                    case Direction.Down: // right wall
                        adjCollider = new Rectangle(collider.X + map.TileWidth - WALL_BORDER, collider.Y - map.TileWidth + WALL_BORDER, WALL_BORDER, collider.Height + map.TileWidth - WALL_BORDER);
                        break;
                    case Direction.Left: // down wall
                        adjCollider = new Rectangle(collider.X - map.TileWidth + WALL_BORDER, collider.Y + map.TileWidth - WALL_BORDER, collider.Width + map.TileWidth - WALL_BORDER + map.TileWidth - WALL_BORDER, WALL_BORDER);
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

        private MindMap.TileOri analyzeTile(MindMap.TileKind tk, Direction dir) {
            if (tk == MindMap.TileKind.Wall) {
                switch (dir) {
                    case Direction.Up:
                        return MindMap.TileOri.Left;
                    case Direction.Right:
                        return MindMap.TileOri.Up;
                    case Direction.Down:
                        return MindMap.TileOri.Right;
                    case Direction.Left:
                        return MindMap.TileOri.Down;
                }
            } else if (tk == MindMap.TileKind.Corner) {
                switch (dir) {
                    case Direction.Up:
                        return MindMap.TileOri.DownLeft;
                    case Direction.Right:
                        return MindMap.TileOri.UpLeft;
                    case Direction.Down:
                        return MindMap.TileOri.UpRight;
                    case Direction.Left:
                        return MindMap.TileOri.DownRight;
                }
            }

            throw new FormatException("unrecognized tile");
        }
    }
}