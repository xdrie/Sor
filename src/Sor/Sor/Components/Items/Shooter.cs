using Glint.Sprites;
using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Components.Items {
    public class Shooter : GAnimatedSprite {
        public Shooter() : base(Core.Content.LoadTexture("Data/sprites/shoot.png"), 128, 128) { }

        public BoxCollider hitbox;
        public bool firing = false;
        private Vector2 hitboxOffset;

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
            // set up hitbox
            hitbox = new BoxCollider(-2 * 2, -28 * 2, 4 * 2, 20 * 2) {Tag = Constants.Colliders.SHOOT};
            hitboxOffset = new Vector2(0, -28);
            
            hitbox.IsTrigger = true;
            Flags.SetFlagExclusive(ref hitbox.PhysicsLayer, Constants.Physics.LAYER_FIRE);
            Entity.AddComponent(hitbox);

            onAnimCompleted(null); // reset animation
        }

        public void prepare() {
            firing = true;
            hitbox.LocalOffset = hitboxOffset;
        }

        private void onAnimCompleted(string anim) {
            animator.Play("idle");
            firing = false;
            hitbox.LocalOffset = -hitboxOffset;
        }

        public void destroy() {
            // destroy shoot after done
            Entity.RemoveComponent(this);
        }
    }
}