using Glint.Sprites;
using Nez;

namespace Sor.Components.Items {
    public class Shoot : GAnimatedSprite {
        public Shoot() : base(Core.Content.LoadTexture("Data/sprites/shoot.png"), 64, 64) { }

        public override void Initialize() {
            base.Initialize();

            animator.AddAnimation("fire", new[] {
                sprites[0],
                sprites[1],
                sprites[2],
                sprites[3],
                sprites[4],
                sprites[5],
                sprites[6]
            });
        }
    }
}