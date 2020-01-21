using Glint.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Units;

namespace Sor.Components.Things {
    public class Capsule : GAnimatedSprite, IUpdatable {
        private float flashSpeed = 10f;
        public int energy = 10;

        public Capsule() : base(Core.Content.LoadTexture("Data/sprites/nrg.png"), 16, 16) {
            animator.AddAnimation("default", new[] {sprites[0], sprites[1], sprites[2], sprites[3]});
            
            animator.Play("default");
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            Entity.AddComponent<CapsuleBody>();
            Entity.AddComponent(new BoxCollider(-4, -6, 4, 12){Tag = Constants.TAG_THING_COLLIDER});
        }

        public void Update() {
            var alpha = (int) Mathf.Sin(Time.DeltaTime / flashSpeed) * 155 + 100;
            animator.Color = new Color(animator.Color.R, animator.Color.G, animator.Color.B, alpha);
        }
        
        class CapsuleBody : KinBody {
            private float maxAngularFloat = Mathf.PI * 1.4f;
            private float maxLinearFloat = 20f;
            public override void Initialize() {
                base.Initialize();

                maxAngular = maxAngularFloat;
                angularVelocity = Nez.Random.Range(-1f, -1f) * maxAngularFloat;
                maxVelocity = new Vector2(maxLinearFloat);
                velocity = Nez.Random.Range(new Vector2(-maxLinearFloat), new Vector2(maxLinearFloat));
            }
        }

        public void destroy() {
            // TODO: trigger some sort of more interesting animation
            Entity.Destroy();
        }
    }
}