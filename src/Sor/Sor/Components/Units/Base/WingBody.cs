using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Sor.Components.Input;
using Sor.Components.Things;

namespace Sor.Components.Units {
    public class WingBody : KinBody, ITriggerListener {
        public Wing me;
        private InputController controller;

        private GameContext gameContext;

        public float turnPower = Mathf.PI * 0.32f;
        public float thrustPower = 2f;
        public float boostFactor = 6.2f;
        private const float VELOCITY_REDUCTION_EXP = 0.98f;

        public float boostCooldown = 0f;
        private double boostDrain = 400;

        public override void Initialize() {
            base.Initialize();

            gameContext = Core.Services.GetService<GameContext>();
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            me = Entity.GetComponent<Wing>();
            controller = Entity.GetComponent<InputController>();

            maxAngular = turnPower * 2f;
            angularDrag = turnPower * 2f;
            drag = new Vector2(16f);
            maxVelocity = new Vector2(80f);
        }

        public override void Update() {
            base.Update();

            if (controller != null) {
                movement();
                interaction();
            }
        }

        private void interaction() {
            if (controller.tetherInput.IsPressed) {
                var capEnergy = 400;
                var capSpeed = 40f;
                if (me.core.energy > capEnergy) {
                    me.core.energy -= capEnergy;
                    // shoot out a capsule
                    var thrustVec = new Vector2(0, -capSpeed);
                    var capNt = Entity.Scene.CreateEntity(null, Entity.Position);
                    var cap = capNt.AddComponent<Capsule>();
                    cap.firstAvailableAt = Time.TimeSinceSceneLoad + 2f;
                    var capBody = cap.launch(capEnergy, thrustVec.rotate(angle));
                }
            }
        }

        protected override void applyMotion(Vector2 posDelta) {
            var motion = posDelta;
            var moveCollisions = new List<CollisionResult>();
            mov.AdvancedCalculateMovement(ref motion, moveCollisions);
            foreach (var result in moveCollisions) {
                // collision with a wall
                if (result.Collider?.Tag == Constants.COLLIDER_WALL) {
                    // suck velocity from hitting the wall
                    velocity *= VELOCITY_REDUCTION_EXP;
                    motion -= result.MinimumTranslationVector;
                }
                // collision with another ship
                else if (result.Collider?.Tag == Constants.COLLIDER_SHIP) {
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

            mov.ApplyMovement(motion);
        }

        private void movement() {
            // apply turn input
            var turnInput = controller.moveDirectionInput.Value.X;
            angularVelocity += turnInput * turnPower;

            // get thrust input
            var thrustInput = controller.moveDirectionInput.Value.Y;
            var thrustVal = thrustPower;

            // boost ribbon
            var boostRibbon = Entity.GetComponent<TrailRibbon>();
            // var trail = Entity.GetComponent<SpriteTrail>();
            if (controller.boostInput && Time.TotalTime > boostCooldown) {
                var runDrain = boostDrain * Time.DeltaTime; // boosting drains energy
                if (me.core.energy > runDrain) {
                    me.core.energy -= runDrain;
                    // boost the ship
                    thrustVal *= boostFactor; // multiply thrust power
                    maxVelocity = new Vector2(440f); // increase velocity cap
                    if (gameContext.config.maxVfx) {
                        Entity.Scene.Camera.GetComponent<CameraShake>().Shake(10f, 0.85f);
                    }

                    if (controller.boostInput.IsPressed) {
                        if (!boostRibbon.IsEmitting) {
                            boostRibbon.StartEmitting();
                            boostRibbon.Enabled = true;
                        }

                        // trail.EnableSpriteTrail();
                    }
                }
            }
            else {
                if (controller.boostInput.IsReleased) { // when boost stopped, set a cooldown
                    boostCooldown = Time.TotalTime + Constants.BOOST_COOLDOWN;
                    if (boostRibbon.IsEmitting) {
                        boostRibbon.StopEmitting();
                    }

                    // trail.DisableSpriteTrail();
                }

                maxVelocity = new Vector2(80f); // reset velocity cap
            }

            // forward thrust
            if (thrustInput <= 0) {
                var thrustVec = new Vector2(0, thrustInput * thrustVal);
                velocity += thrustVec.rotate(angle);
            }
            else { // slowdown thrust
                float fac = VELOCITY_REDUCTION_EXP + (1 - VELOCITY_REDUCTION_EXP) * (1 - thrustInput);
                velocity *= fac;
            }
        }

        public void OnTriggerEnter(Collider other, Collider local) {
            if (other.Tag == Constants.COLLIDER_THING) {
                var hitEntity = other.Entity;
                if (hitEntity.HasComponent<Capsule>()) {
                    var capsule = hitEntity.GetComponent<Capsule>();
                    if (Time.TimeSinceSceneLoad > capsule.firstAvailableAt) {
                        // apply the capsule
                        me.core.energy += capsule.energy;
                        capsule.energy = 0;
                        capsule.destroy(); // blow it up
                    }
                }
            }
        }

        public void OnTriggerExit(Collider other, Collider local) {
            // ...
        }
    }
}