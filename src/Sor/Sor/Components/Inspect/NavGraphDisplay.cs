using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Nav;

namespace Sor.Components.Inspect {
#if DEBUG
    public class NavGraphDisplay : RenderableComponent {
        private readonly MapRepr mapRepr;
        private readonly TiledMapRenderer mapRenderer;

        public override RectangleF Bounds => mapRenderer.Bounds;

        public NavGraphDisplay(MapRepr mapRepr, TiledMapRenderer mapRenderer) {
            this.mapRepr = mapRepr;
            this.mapRenderer = mapRenderer;
        }

        public override void Render(Batcher batcher, Camera camera) {
            // draw points for each point on the map graph
            foreach (var node in mapRepr.sng.nodes) {
                Vector2 toWorldPos(Point tilePos) => mapRenderer.TiledMap.TileToWorldPosition(tilePos.ToVector2());
                batcher.DrawHollowRect(toWorldPos(node.pos), 4f, 4f, Color.GreenYellow, 1f);
                // draw edges
                foreach (var adj in node.links) {
                    batcher.DrawLine(toWorldPos(node.pos), toWorldPos(adj.pos), Color.GreenYellow, 1f);
                }
            }
        }
    }
#endif
}