using System.Collections.Generic;
using Glint.Physics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Sor.AI.Signals;
using Sor.Components.Input;
using Sor.Components.Items;
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
        public float baseCapSpeed = 40f;

        // - interaction state
        public float shootCooldown = 0f;
        public float shootCharge = 0f;

        // - physiology
        public float metabolicRate; // energy burn per-second
        private float boostDrainKg = 100; // boost drain per kg

        // - debug properties
        public bool intangible = false;

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

            me.hitbox.Enabled = !intangible;

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
            if (controller.tetherInput.IsDown) {
                // increase shoot charge
                shootCharge += Time.DeltaTime;
            }

            if (shootCharge > 0 && controller.tetherInput.IsReleased) {
                // clamp shoot charge
                shootCharge = Mathf.Clamp(shootCharge, 0f, 2f);
                var capEnergy = Constants.Mechanics.CAPSULE_SIZE * Mathf.Pow(1 + shootCharge, 2f);
                var capSpeed = baseCapSpeed * Mathf.Pow(1 + shootCharge, 2f);
                if (me.core.energy > capEnergy) {
                    me.core.energy -= capEnergy;
                    // shoot out a capsule
                    var capMotion = new Vector2(0, -capSpeed);
                    var capNt = Entity.Scene.CreateEntity("pod", Entity.Position)
                        .SetTag(Constants.Tags.THING);
                    var cap = capNt.AddComponent<Capsule>();
                    cap.firstAvailableAt = Time.TotalTime + 1f;
                    cap.interactor = me;
                    cap.launch(capEnergy, capMotion.Rotate(angle));
                }

                shootCharge = 0; // reset charge
            }

            if (controller.fireInput.IsPressed) {
                // check if entity has a gun
                var shootEnergy = Constants.Mechanics.SHOOT_COST_PER_KG * mass;
                var gun = Entity.GetComponent<Shooter>();
                if (gun != null && Time.TotalTime > shootCooldown) {
                    // ensure gun can fire
                    if (me.core.energy > shootEnergy) {
                        me.core.energy -= shootEnergy;
                        gun.prepare();
                        gun.animator.Play("fire", SpriteAnimator.LoopMode.Once);
                        shootCooldown = Time.TotalTime + Constants.Mechanics.SHOOT_COOLDOWN;
                    }
                }
            }
        }

        protected override void applyMotion(Vector2 posDelta) {
            var motion = posDelta;
            if (intangible) {
                mov.ApplyMovement(motion);
                return;
            }

            var moveCollisions = new List<CollisionResult>();
            var calcMotion = motion; // a dummy motion
            mov.AdvancedCalculateMovement(ref calcMotion, moveCollisions);
            foreach (var result in moveCollisions) {
                // collision with a wall
                if (!boosting && result.Collider?.Tag == Constants.Colliders.WALL) {
                    // suck velocity from hitting the wall
                    velocity *= VELOCITY_REDUCTION_EXP;
                    motion -= result.MinimumTranslationVector;
                }
                // collision with another ship
                else if (result.Collider?.Tag == Constants.Colliders.SHIP) {
                    var hitShip = result.Collider.Entity.GetComponent<WingBody>();
                    // conserve momentum in the collision
                    var impactMomentum = hitShip.momentum; // store impact
                    var netMomentum = momentum + hitShip.momentum;
                    var totalMass = mass + hitShip.mass;
                    var vf = netMomentum / totalMass;
                    velocity = vf;
                    if (!hitShip.intangible) {
                        hitShip.velocity = vf;
                    }

                    motion -= result.MinimumTranslationVector;

                    // send signal to mind
                    if (me.mind.control) {
                        me.mind.signal(new PhysicalSignals.BumpSignal(hitShip.me, impactMomentum));
                    }
                }
            }

            // apply our manually adjusted motion
            mov.ApplyMovement(motion);
        }

        private void movement() {
            // apply turn input
            var turnInput = controller.moveTurnInput.Value;
            angularVelocity += turnInput * turnPower;

            // get thrust input
            var thrustInput = controller.moveThrustInput.Value;
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
                if (me.core.overloadedNess() > 0) { // boost bonus when overloaded
                    maxVelocity *= Mathf.Sqrt(1 + me.core.overloadedNess());
                }

                if (gameContext.config.maxVfx) {
                    Entity.Scene.Camera.GetComponent<CameraShake>().Shake(10f, 0.85f);
                }

                if (!boostRibbon.IsEmitting) {
                    boostRibbon.StartEmitting();
                    boostRibbon.Enabled = true;
                }
            } else {
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
                velocity += thrustVec.Rotate(angle);
            } else { // slowdown thrust
                // float fac = VELOCITY_REDUCTION_EXP + (1 - VELOCITY_REDUCTION_EXP) * (1 - thrustInput);
                // velocity *= fac;
                // var invVelocity = -velocity;
                drag = new Vector2(brakeDrag);
            }
        }

        public void OnTriggerEnter(Collider other, Collider local) {
            var hitEntity = other.Entity;
            switch (other.Tag) {
                case Constants.Colliders.THING: {
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

                    break;
                }
                case Constants.Colliders.SHOOT: {
                    if (hitEntity.HasComponent<Shooter>()) {
                        var shooter = hitEntity.GetComponent<Shooter>();
                        if (shooter.firing) {
                            // ouch
                            me.core.energy -= Constants.Mechanics.SHOOT_DRAIN;
                            me.core.clamp();
                            // send signal to mind
                            if (me.mind.control) {
                                me.mind.signal(new PhysicalSignals.ShotSignal(shooter));
                            }
                        }
                    }

                    break;
                }
            }

            if (other.Tag == Constants.Colliders.LANE) {
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
            if (other.Tag == Constants.Colliders.LANE) {
                drag = new Vector2(baseDrag);
            }
        }
    }
}