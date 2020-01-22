using Glint.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace Sor.Components.UI {
    internal class IndicatorBar : GSprite {
        public SpriteRenderer backdropRenderer;
        public int height;
        public int width;

        public IndicatorBar(int width, int height) : base(Core.Content.LoadTexture("Data/ui/indicator_bar.png")) {
            this.width = width;
            this.height = height;
            backdropRenderer = new SpriteRenderer(texture);
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            var clipRect = new Rectangle(0, 0, width, height);
            spriteRenderer.SetSprite(new Sprite(texture, clipRect, Vector2.Zero));
            backdropRenderer.SetSprite(new Sprite(texture, clipRect, Vector2.Zero));
            backdropRenderer.RenderLayer = spriteRenderer.RenderLayer;
            backdropRenderer.LayerDepth = 1f;
            Entity.AddComponent(backdropRenderer);
        }

        public void setColors(Color fill, Color bg) {
            spriteRenderer.Color = fill;
            backdropRenderer.Color = bg;
        }

        public void setValue(float value) {
            spriteRenderer.SetSprite(new Sprite(texture, new Rectangle(0, 0, (int) (value * width), height),
                Vector2.Zero));
        }
    }
}