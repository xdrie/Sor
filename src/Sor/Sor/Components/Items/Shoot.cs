using Glint.Sprites;
using Nez;
using Nez.Sprites;

namespace Sor.Components.Items {
    public class Shoot : GAnimatedSprite {
        public Shoot() : base(Core.Content.LoadTexture("Data/sprites/shoot.png"), 128, 128) { }

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
            animator.OnAnimationCompletedEvent += animationCompleted;
        }

        private void animationCompleted(string anim) {
            // destroy shoot after done
             Entity.RemoveComponent(this);
        }
    }
}