using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Input;

namespace Sor.Components.Units {
    public class WingBody : KinBody {
        public Wing me;
        private InputController controller;

        public float turnPower = Mathf.PI * 0.32f;
        public float thrustPower = 4f;
        private Mover mov;
        
        private const float VELOCITY_REDUCTION_EXP = 0.98f;

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            me = Entity.GetComponent<Wing>();
            controller = Entity.GetComponent<InputController>();
            mov = Entity.AddComponent<Mover>();

            maxAngular = turnPower * 2f;
            angularDrag = turnPower * 2f;
            drag = new Vector2(thrustPower * 4f);
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
            if (hitbox.CollidesWithAnyMultiple(motion, collisionResults)) {
                foreach (var result in collisionResults) {
                    // collision with a wall
                    if (result.Collider?.Tag == Constants.TAG_WALL_COLLIDER) {
                        // suck velocity from hitting the wall
                        velocity *= VELOCITY_REDUCTION_EXP;
                        motion -= result.MinimumTranslationVector;
                    }
                    // collision with another ship
                    else if (result.Collider?.Tag == Constants.TAG_SHIP_COLLIDER) {
                        var hitShip = result.Collider.Entity.GetComponent<WingBody>();
                        // conserve momentum in the collision
                        var netMomentum = momentum + hitShip.momentum;
                        var totalMass = mass + hitShip.mass;
                        var vf = netMomentum / totalMass;
                        velocity = vf;
                        hitShip.velocity = vf;
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
                float fac = VELOCITY_REDUCTION_EXP + (1 - VELOCITY_REDUCTION_EXP) * (1 - thrustInput);
                velocity *= fac;
            }
        }
    }
}