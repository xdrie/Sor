using LunchtimeGears.Calc;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Units;

namespace Sor.Components.Things {
    public class Capsule : Thing, IUpdatable {
        public float energy = 0;
        public bool acquired = false;
        public float firstAvailableAt = 0;
        public const float lifetime = 20f;
        public float despawnAt = 0;
        
        public CapsuleBody body;
        public Wing sender = null;
        public Tree creator = null;

        public Capsule() : base(Core.Content.LoadTexture("Data/sprites/nrg.png"), 16, 16) {
            animator.AddAnimation("default", new[] {sprites[0], sprites[1], sprites[2], sprites[3]});

            animator.Play("default");
        }

        public override void Initialize() {
            base.Initialize();

            despawnAt = Time.TotalTime + lifetime;

            body = Entity.AddComponent<CapsuleBody>();
            Entity.AddComponent(new BoxCollider(-8, -12, 8, 24) {Tag = Constants.COLLIDER_THING, IsTrigger = true});
            Entity.AddComponent(new BoxCollider(-40, -40, 80, 80) {Tag = Constants.TRIGGER_GRAVITY, IsTrigger = true});
            
            // use slow updates
            UpdateInterval = 10;
        }

        public void launch(float launchEnergy, Vector2 launch) {
            energy = launchEnergy;
            body.velocity += launch;
        }

        public class CapsuleBody : KinBody {
            private float maxAngularFloat = Mathf.PI * 1.4f;
            private float maxLinearFloat = 40f;
            private float randomLinearFloat = 10f;

            public override void Initialize() {
                base.Initialize();

                maxAngular = maxAngularFloat;
                angularVelocity = Random.Range(-1f, 1f) * maxAngularFloat;
                maxVelocity = new Vector2(maxLinearFloat);
                velocity = Random.Range(new Vector2(-randomLinearFloat), new Vector2(randomLinearFloat));
            }
        }

        public void acquire() {
            if (!acquired) {
                energy = 0;
                var tw = spriteRenderer.TweenColorTo(Color.Black, 0.4f);
                tw.SetCompletionHandler(t => { Entity?.Destroy(); });
                tw.Start();
            }
            acquired = true;
        }

        public void Update() {
            // update animation speed based on energy
            var animSpeed = Mathf.Clamp(energy / 400f, 0.5f, 2f);
            animator.Speed = animSpeed;
            // check despawn
            if (Time.TotalTime > despawnAt) {
                Enabled = false;
                Entity.Destroy();
            }
        }
    }
}