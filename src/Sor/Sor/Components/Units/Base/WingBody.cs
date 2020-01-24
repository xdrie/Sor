using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Sor.AI.Signals;
using Sor.Components.Input;
using Sor.Components.Things;

namespace Sor.Components.Units {
    public class WingBody : KinBody, ITriggerListener {
        public Wing me;
        private InputController controller;

        private GameContext gameContext;

        public float turnPower = Mathf.PI * 0.72f;
        public float thrustPower = 2f;
        public float boostFactor = 6.2f;
        public float laneFactor = 4f;
        public float topSpeed = 80f;
        public float stdDrag = 16f;
        public float gravityFactor = 4000f;
        private const float VELOCITY_REDUCTION_EXP = 0.98f;

        public float boostCooldown = 0f;
        public bool boosting = false;
        private double boostDrainKg = 100; // boost drain per kg

        public override void Initialize() {
            base.Initialize();

            gameContext = Core.Services.GetService<GameContext>();
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            me = Entity.GetComponent<Wing>();
            controller = Entity.GetComponent<InputController>();
            recalculateKinematics();
        }

        public void recalculateKinematics() {
            maxAngular = turnPower * 1.2f;
            angularDrag = turnPower * 2f;
            drag = new Vector2(stdDrag);
            maxVelocity = new Vector2(topSpeed);
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
                var capEnergy = Constants.CAPSULE_SIZE;
                var capSpeed = 40f;
                if (me.core.energy > capEnergy) {
                    me.core.energy -= capEnergy;
                    // shoot out a capsule
                    var capMotion = new Vector2(0, -capSpeed);
                    var capNt = Entity.Scene.CreateEntity("pod", Entity.Position);
                    var cap = capNt.AddComponent<Capsule>();
                    cap.firstAvailableAt = Time.TotalTime + 1f;
                    cap.sender = me;
                    cap.launch(capEnergy, capMotion.rotate(angle));
                }
            }
        }

        protected override void applyMotion(Vector2 posDelta) {
            var motion = posDelta;
            var moveCollisions = new List<CollisionResult>();
            var calcMotion = motion; // a dummy motion
            mov.AdvancedCalculateMovement(ref calcMotion, moveCollisions);
            foreach (var result in moveCollisions) {
                // collision with a wall
                if (!boosting && result.Collider?.Tag == Constants.COLLIDER_WALL) {
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

            // apply our manually adjusted motion
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
            var boostDrain = boostDrainKg * mass * Time.DeltaTime; // boosting drains energy
            if (controller.boostInput && me.core.energy > boostDrain && Time.TotalTime > boostCooldown) {
                me.core.energy -= boostDrain;
                // boost the ship
                boosting = true;
                thrustVal *= boostFactor; // multiply thrust power
                maxVelocity = new Vector2(440f); // increase velocity cap
                if (gameContext.config.maxVfx) {
                    Entity.Scene.Camera.GetComponent<CameraShake>().Shake(10f, 0.85f);
                }

                if (!boostRibbon.IsEmitting) {
                    boostRibbon.StartEmitting();
                    boostRibbon.Enabled = true;
                }
            }
            else {
                boosting = false;
                maxVelocity = new Vector2(topSpeed); // reset velocity cap
                if (boostRibbon.IsEmitting) {
                    boostRibbon.StopEmitting();
                }

                if (controller.boostInput.IsReleased) { // when boost stopped, set a cooldown
                    boostCooldown = Time.TotalTime + Constants.BOOST_COOLDOWN;
                }
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
                    if (!capsule.acquired && Time.TotalTime > capsule.firstAvailableAt) {
                        // apply the capsule
                        var gotEnergy = capsule.energy;
                        me.core.energy += gotEnergy;
                        capsule.acquire(); // blow it up
                        // send signal to mind
                        if (me.mind.control) {
                            me.mind.signal(new ItemSignals.CapsuleAcquiredSignal(capsule, gotEnergy));
                        }
                    }
                }
            }

            if (other.Tag == Constants.COLLIDER_LANE) {
                // lanes multiply velocity
                velocity *= laneFactor;
                drag = Vector2.Zero;
            }

            if (other.Tag == Constants.TRIGGER_GRAVITY) {
                var gravThing = other.Entity;
                var succ = true;
                if (gravThing.HasComponent<Capsule>()) {
                    var cap = gravThing.GetComponent<Capsule>();
                    if (Time.TotalTime < cap.firstAvailableAt) {
                        succ = false;
                    }
                }

                if (succ) {
                    var thingBody = gravThing.GetComponent<KinBody>();
                    var toMe = Entity.Position - gravThing.Position;
                    var toMeDir = Vector2Ext.Normalize(toMe);
                    var dist = toMe.Length();
                    var gravForce = (gravityFactor * mass) / (dist * dist);
                    thingBody.velocity += gravForce * toMeDir;
                }
            }
        }

        public void OnTriggerExit(Collider other, Collider local) {
            if (other.Tag == Constants.COLLIDER_LANE) {
                drag = new Vector2(stdDrag);
            }
        }
    }
}