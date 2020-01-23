using Glint.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Units;

namespace Sor.Components.Things {
    public class Capsule : Thing, IUpdatable {
        public double energy = 0;
        public bool acquired = false;
        public float firstAvailableAt = 0;
        public const float lifetime = 20f;
        public float despawnAt = 0;
        public Wing sender = null;
        public Tree creator = null;

        public Capsule() : base(Core.Content.LoadTexture("Data/sprites/nrg.png"), 16, 16) {
            animator.AddAnimation("default", new[] {sprites[0], sprites[1], sprites[2], sprites[3]});

            animator.Play("default");
        }

        public override void Initialize() {
            base.Initialize();

            despawnAt = Time.TotalTime + lifetime;

            Entity.AddComponent<CapsuleBody>();
            Entity.AddComponent(new BoxCollider(-8, -12, 8, 24) {Tag = Constants.COLLIDER_THING, IsTrigger = true});
            Entity.AddComponent(new BoxCollider(-40, -40, 80, 80) {Tag = Constants.TRIGGER_GRAVITY, IsTrigger = true});
            
            // use slow updates
            // Entity.UpdateInterval = 10;
        }

        public CapsuleBody launch(int energy, Vector2 launch) {
            var capBody = this.GetComponent<CapsuleBody>();
            capBody.velocity += launch;
            return capBody;
        }

        public class CapsuleBody : KinBody {
            private float maxAngularFloat = Mathf.PI * 1.4f;
            private float maxLinearFloat = 40f;
            private float randomLinearFloat = 10f;

            public override void Initialize() {
                base.Initialize();

                maxAngular = maxAngularFloat;
                angularVelocity = Nez.Random.Range(-1f, 1f) * maxAngularFloat;
                maxVelocity = new Vector2(maxLinearFloat);
                velocity = Nez.Random.Range(new Vector2(-randomLinearFloat), new Vector2(randomLinearFloat));
            }
        }

        public void acquire() {
            if (!acquired) {
                energy = 0;
                var tw = spriteRenderer.TweenColorTo(Color.Black, 0.4f);
                tw.SetCompletionHandler(t => { Entity.Destroy(); });
                tw.Start();
            }
            acquired = true;
        }

        public void Update() {
            if (Time.TotalTime > despawnAt) {
                Enabled = false;
                Entity.Destroy();
            }
        }
    }
}