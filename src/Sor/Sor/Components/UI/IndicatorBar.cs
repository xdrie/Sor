using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace Sor.Components.UI {
    public class IndicatorBar : RenderableComponent {
        private Texture2D texture;
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer backdropRenderer;
        public int height;
        public int width;
        public Color bgColor => backdropRenderer.Color;
        public Color fgColor => spriteRenderer.Color;
        public Color overflowColor;
        public int overflowSize = 2;

        public float value;

        public override RectangleF Bounds {
            get {
                if (spriteRenderer.Entity == null) return new RectangleF();
                return spriteRenderer.Bounds;
            }
        }

        public IndicatorBar(int width, int height) {
            this.width = width;
            this.height = height;
            texture = Core.Content.LoadTexture("Data/ui/el/bar.png");
        }

        public override void Initialize() {
            base.Initialize();

            spriteRenderer = Entity.AddComponent(new SpriteRenderer(texture));
            backdropRenderer = Entity.AddComponent(new SpriteRenderer(texture));
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            var clipRect = new Rectangle(0, 0, width, height);
            spriteRenderer.SetSprite(new Sprite(texture, clipRect, Vector2.Zero));
            backdropRenderer.SetSprite(new Sprite(texture, clipRect, Vector2.Zero));
            RenderLayer = backdropRenderer.RenderLayer = spriteRenderer.RenderLayer;
            backdropRenderer.LayerDepth = 1f;
        }

        public void setColors(Color bg, Color fill, Color? overflow) {
            backdropRenderer.Color = bg;
            spriteRenderer.Color = fill;
            if (!overflow.HasValue) {
                overflow = fill;
            }

            overflowColor = overflow.Value;
        }

        public void setValue(float value) {
            this.value = value;
            var barWidthVal = Mathf.Clamp01(this.value);
            spriteRenderer.SetSprite(new Sprite(texture, new Rectangle(0, 0, (int) (barWidthVal * width), height),
                Vector2.Zero));
        }

        public override void Render(Batcher batcher, Camera camera) {
            if (value > 1) {
                var maxLayers = height / overflowSize;
                var remainingValue = value - 1f;
                var layers = Mathf.CeilToInt(remainingValue);
                layers = Math.Min(layers, maxLayers);
                for (int i = 0; i < layers; i++) {
                    var layerValue = Math.Min(1f, remainingValue);
                    batcher.DrawRect(Entity.Position.X, Entity.Position.Y + (i * overflowSize),
                        (int) (layerValue * width), overflowSize, overflowColor);
                    remainingValue -= 1f;
                }
            }
        }
    }
}