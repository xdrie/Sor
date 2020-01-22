using Glint.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace Sor.Components.UI {
    internal class IndicatorBar : GSprite {
        public SpriteRenderer backdropRenderer;
        public int height;
        public int width;

        public IndicatorBar(int width, int height) : base(
            Core.Content.LoadTexture("Data/ui/indicator_bar.png")) {
            this.width = width;
            this.height = height;
            backdropRenderer = new SpriteRenderer(texture);
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            spriteRenderer.SetSprite(new Sprite(texture, new Rectangle(0, 0, width, height), Vector2.Zero));
            backdropRenderer.SetSprite(sprite);
            backdropRenderer.RenderLayer = spriteRenderer.RenderLayer;
            backdropRenderer.LayerDepth = 1f;
            Entity.AddComponent(backdropRenderer);
        }

        public void setValue(float value) {
            spriteRenderer.SetSprite(new Sprite(texture, new Rectangle(0, 0, (int) (value * width), height),
                Vector2.Zero));
        }
    }
}