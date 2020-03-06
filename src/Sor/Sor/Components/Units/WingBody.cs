using System.Collections.Generic;
using Glint.Physics;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Signals;
using Sor.Components.Input;
using Sor.Components.Things;

namespace Sor.Components.Units {
    /// <summary>
    /// Represents the physical body of a bird
    /// </summary>
    public class WingBody : KinBody, ITriggerListener {
        public Wing me;
        private InputController controller;

        private GameContext gameContext;

        // - movement
        public float turnPower = Constants.Physics.DEF_TURN_POWER;
        public float thrustPower = Constants.Physics.DEF_THRUST_POWER;
        public float topSpeed = Constants.Physics.DEF_TOP_SPEED;
        public float boostFactor = Constants.Physics.DEF_BOOST_FACTOR;
        public float boostTopSpeed = Constants.Physics.DEF_BOOST_TOP_SPEED;
        public float baseDrag = Constants.Physics.DEF_BASE_DRAG;
        public float brakeDrag = Constants.Physics.DEF_BRAKE_DRAG;
        private const float VELOCITY_REDUCTION_EXP = 0.98f;
        
        // - movement state
        public float boostCooldown = 0f;
        public bool boosting = false;
        
        // - interaction
        public float laneFactor = 4f; // speed boost from touching lanes
        public float gravityFactor = 4000f;
        
        // - physiology
        public float metabolicRate; // energy burn per-second
        private float boostDrainKg = 100; // boost drain per kg

        public override void Initialize() {
            base.Initialize();

            gameContext = Core.Services.GetService<GameContext>();
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            me = Entity.GetComponent<Wing>();
            controller = Entity.GetComponent<InputController>();
            recalculateValues();
        }

        public void recalculateValues() {
            maxAngular = turnPower * 1.2f;
            angularDrag = turnPower * 2f;
            drag = new Vector2(baseDrag);
            maxVelocity = new Vector2(topSpeed);

            metabolicRate = Constants.Mechanics.CALORIES_PER_KG * mass;
        }

        public override void Update() {
            base.Update();
            
            // metabolism
            if (me.core.energy > 0) {
                me.core.energy -= metabolicRate * Time.DeltaTime;
            }

            if (controller != null) {
                movement();
                interaction();
            }
        }

        private void interaction() {
            if (controller.tetherInput.IsPressed) {
                var capEnergy = Constants.Mechanics.CAPSULE_SIZE;
                var capSpeed = 40f;
                if (me.core.energy > capEnergy) {
                    me.core.energy -= capEnergy;
                    // shoot out a capsule
                    var capMotion = new Vector2(0, -capSpeed);
                    var capNt = Entity.Scene.CreateEntity("pod", Entity.Position)
                        .SetTag(Constants.Tags.ENTITY_THING);
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
                if (!boosting && result.Collider?.Tag == Constants.Colliders.COLLIDER_WALL) {
                    // suck velocity from hitting the wall
                    velocity *= VELOCITY_REDUCTION_EXP;
                    motion -= result.MinimumTranslationVector;
                }
                // collision with another ship
                else if (result.Collider?.Tag == Constants.Colliders.COLLIDER_SHIP) {
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
                maxVelocity = new Vector2(boostTopSpeed); // increase velocity cap
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
                    boostCooldown = Time.TotalTime + Constants.Mechanics.BOOST_COOLDOWN;
                }
            }

            drag = new Vector2(baseDrag);
            // forward thrust
            if (thrustInput <= 0) {
                var thrustVec = new Vector2(0, thrustInput * thrustVal * Time.DeltaTime);
                velocity += thrustVec.rotate(angle);
            }
            else { // slowdown thrust
                // float fac = VELOCITY_REDUCTION_EXP + (1 - VELOCITY_REDUCTION_EXP) * (1 - thrustInput);
                // velocity *= fac;
                // var invVelocity = -velocity;
                drag = new Vector2(brakeDrag);
            }
        }

        public void OnTriggerEnter(Collider other, Collider local) {
            if (other.Tag == Constants.Colliders.COLLIDER_THING) {
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

            if (other.Tag == Constants.Colliders.COLLIDER_LANE) {
                // lanes multiply velocity
                velocity *= laneFactor;
                drag = Vector2.Zero;
            }

            if (other.Tag == Constants.Mechanics.TRIGGER_GRAVITY) {
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
                    var velInfluence = Mathf.Clamp(velocity.Length(), 1f, 10f);
                    var gravForce = (gravityFactor * mass * velInfluence) / (dist * dist);
                    thingBody.velocity += gravForce * toMeDir;
                }
            }
        }

        public void OnTriggerExit(Collider other, Collider local) {
            if (other.Tag == Constants.Colliders.COLLIDER_LANE) {
                drag = new Vector2(baseDrag);
            }
        }
    }
}