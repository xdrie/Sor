using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;

namespace Sor.Game {
    public class MapLoader {
        private Scene scene;
        private readonly Entity mapEntity;

        public MapLoader(Scene scene, Entity mapEntity) {
            this.scene = scene;
            this.mapEntity = mapEntity;
        }

        public void load(TmxMap map) {
            var structure = map.GetLayer<TmxLayer>("structure");
            // loop through colliders
            var colliders = structure.GetCollisionRectangles();
            var adjustedColliders = new List<Rectangle>();
            foreach (var collider in colliders) {
                var colliderCenter = new Vector2(collider.Left + collider.Width / 2, collider.Top + collider.Height / 2);
                var corrTilePos = map.WorldToTilePosition(colliderCenter);
                var corrTile = structure.GetTile(corrTilePos.X, corrTilePos.Y);
                var corrDirection = tileDirection(corrTile);
                var adjCollider = default(Rectangle);
                var wallBorder = 1;
                // adjust collider based on direction
                switch (corrDirection) {
                    case Direction.Up: // left wall
                        adjCollider = new Rectangle(collider.X, collider.Y - map.TileWidth + wallBorder, wallBorder, collider.Height + map.TileWidth - wallBorder);
                        break;
                    case Direction.Right: // top wall
                        adjCollider = new Rectangle(collider.X, collider.Y, collider.Width, wallBorder);
                        break;
                    case Direction.Down: // right wall
                        adjCollider = new Rectangle(collider.X + map.TileWidth - wallBorder, collider.Y - map.TileWidth + wallBorder, wallBorder, collider.Height + map.TileWidth - wallBorder);
                        break;
                    case Direction.Left: // down wall
                        adjCollider = new Rectangle(collider.X - map.TileWidth + wallBorder, collider.Y + map.TileWidth - wallBorder, collider.Width + map.TileWidth - wallBorder + map.TileWidth - wallBorder, wallBorder);
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

        public Direction tileDirection(TmxLayerTile tile) {
            var dir = Direction.Up;
            if (tile.DiagonalFlip && tile.HorizontalFlip && !tile.VerticalFlip)
                dir = Direction.Right;

            if (tile.DiagonalFlip && !tile.HorizontalFlip && tile.VerticalFlip)
                dir = Direction.Left;

            if (!tile.DiagonalFlip && tile.HorizontalFlip && tile.VerticalFlip)
                dir = Direction.Down;

            return dir;
        }
    }
}