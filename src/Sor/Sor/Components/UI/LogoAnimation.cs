using Glint.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Sor.Components.UI {
    internal class LogoAnimation : GAnimatedSprite {
        public LogoAnimation() : base(Core.Content.LoadTexture("Data/img/khnpr.png"), 32, 32) { }

        public override void Initialize() {
            base.Initialize();

            animator.AddAnimation("in", new[] {
                sprites[0],
                sprites[1],
                sprites[2],
                sprites[3]
            }, 10);
        }
    }
}