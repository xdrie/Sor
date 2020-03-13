using Glint.Sprites;
using Nez;

namespace Sor.Components.Items {
    public class Shooter : GAnimatedSprite {
        public Shooter() : base(Core.Content.LoadTexture("Data/sprites/shoot.png"), 128, 128) { }

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
            animator.AddAnimation("fire", new[] {
                sprites[0],
                sprites[1],
                sprites[2],
                sprites[3],
                sprites[4],
                sprites[5],
                sprites[6]
            });
            animator.AddAnimation("idle", new[] {
                sprites[7],
                sprites[8],
                sprites[9],
                sprites[10],
            });

            animator.OnAnimationCompletedEvent += onAnimCompleted;
            onAnimCompleted(null); // reset animation
        }

        private void onAnimCompleted(string anim) {
            animator.Play("idle");
        }

        public void destroy() {
            // destroy shoot after done
            Entity.RemoveComponent(this);
        }
    }
}