using Glint.Sprites;
using Nez;

namespace Sor.Components.UI {
    internal class DevLogoAnimation : GAnimatedSprite {
        public DevLogoAnimation() : base(Core.Content.LoadTexture("Data/img/devlogo.png"), 32, 32) { }

        public override void Initialize() {
            base.Initialize();

            // animator.AddAnimation("in", new[] {
            //     sprites[0],
            //     sprites[1],
            //     sprites[2],
            //     sprites[3]
            // }, 10);
        }
    }
}