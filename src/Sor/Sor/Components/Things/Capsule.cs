using Glint.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Units;

namespace Sor.Components.Things {
    public class Capsule : GAnimatedSprite, IUpdatable {
        private float flashSpeed = 10f;
        public double energy = 0;
        public bool acquired = false;
        public float firstAvailableAt = 0;
        public Wing sender = null;
        public Tree creator = null;

        public Capsule() : base(Core.Content.LoadTexture("Data/sprites/nrg.png"), 16, 16) {
            animator.AddAnimation("default", new[] {sprites[0], sprites[1], sprites[2], sprites[3]});

            animator.Play("default");
        }

        public override void Initialize() {
            base.Initialize();

            Entity.AddComponent<CapsuleBody>();
            Entity.AddComponent(new BoxCollider(-8, -12, 8, 24) {Tag = Constants.COLLIDER_THING, IsTrigger = true});
            Entity.AddComponent(new BoxCollider(-40, -40, 80, 80) {Tag = Constants.TRIGGER_GRAVITY, IsTrigger = true});
        }

        public CapsuleBody launch(int energy, Vector2 launch) {
            var capBody = this.GetComponent<CapsuleBody>();
            capBody.velocity += launch;
            return capBody;
        }

        public void Update() {
            var alpha = (int) Mathf.Sin(Time.DeltaTime / flashSpeed) * 155 + 100;
            animator.Color = new Color(animator.Color.R, animator.Color.G, animator.Color.B, alpha);
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
                var tw = spriteRenderer.TweenColorTo(Color.Black, 0.4f);
                tw.SetCompletionHandler(t => { Entity.Destroy(); });
                tw.Start();
            }
            acquired = true;
        }
    }
}