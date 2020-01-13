using System.Collections.Generic;
using Glint.Physics;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Input;

namespace Sor.Components.Units {
    public class ShipBody : KinBody {
        public Ship me;
        private InputController controller;

        public float turnPower = Mathf.PI / 4f;
        public float thrustPower = 4f;
        private Mover mov;

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            me = Entity.GetComponent<Ship>();
            controller = Entity.GetComponent<InputController>();
            mov = Entity.AddComponent<Mover>();

            maxAngular = turnPower * 2f;
            angularDrag = turnPower * 2f;
            drag = new Vector2(thrustPower / 1f);
            maxVelocity = new Vector2(thrustPower * 20f);
        }

        public override void Update() {
            base.Update();

            if (controller != null) {
                movement();
            }
        }

        protected override Vector2 motion(Vector2 posDelta) {
            var motion = base.motion(posDelta);

            var collisionResults = new List<CollisionResult>();
            var hitbox = Entity.GetComponent<BoxCollider>();
            if (hitbox.collidesWithAnyMultiple(motion, collisionResults)) {
                foreach (var result in collisionResults) {
                    // suck velocity when hitting a wall
                    if (result.Collider.Tag == Constants.TAG_WALL_COLLIDER) {
                        // apply adjustment
                        velocity *= 0.8f;
                        motion -= result.MinimumTranslationVector;
                    }
                }
            }

            return motion;
        }

        private void movement() {
            var turnInput = controller.moveDirectionInput.Value.X;
            angularVelocity += turnInput * turnPower;
            var thrustInput = controller.moveDirectionInput.Value.Y;
            if (thrustInput <= 0) {
                var thrustVec = new Vector2(0, thrustInput * thrustPower);
                velocity += thrustVec.rotate(angle);
            }
            else {
                // thrust input is slowdown
                float keepPor = 0.97f;
                float fac = keepPor + (1 - keepPor) * (1 - thrustInput);
                velocity *= fac;
            }
        }
    }
}