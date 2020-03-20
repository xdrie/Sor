using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Components.Inspect {
#if DEBUG
    public class NavGraphDisplay : RenderableComponent {
        private readonly TiledMapRenderer mapRenderer;

        public override RectangleF Bounds => mapRenderer.Bounds;

        public NavGraphDisplay(TiledMapRenderer mapRenderer) {
            this.mapRenderer = mapRenderer;
        }

        public override void Render(Batcher batcher, Camera camera) { }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);
            var mapRepr = NGame.context.map;
            if (mapRepr == null) return;
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