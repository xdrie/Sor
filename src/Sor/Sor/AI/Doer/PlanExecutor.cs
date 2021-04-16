using System;
using Ducia;
using Ducia.Game;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Plans;
using Sor.Components.Input;
using Sor.Components.Units;

namespace Sor.AI.Doer {
    public class PlanExecutor {
        private readonly DuckMind mind;
        private Wing me => mind.state.me;

        private LogicInputController _controller;

        private LogicInputController controller {
            get {
                if (_controller == null) {
                    _controller = mind.entity.GetComponent<LogicInputController>();
                }

                return _controller;
            }
        }

        public PlanExecutor(DuckMind mind) {
            this.mind = mind;
        }

        public void process() {
            // zero out buttons
            controller.zeroButtons();

            // vars for tracking target/goal
            var navTargetSource = default(TargetSource);
            var immediateGoal = false; // whether we've obtained an immediate goal
            while (mind.state.plan.Count > 0 && !immediateGoal) {
                // go through goals until we find one to execute
                var takeNextResult = mind.state.plan.TryPeek(out var nextTask);
                if (!takeNextResult || nextTask == null) return; // tasks changed
                // dequeue completed goals
                var nextTaskStatus = nextTask.status();
                bool continueWithPlan = true;
                switch (nextTaskStatus) {
                    case PlanTask.Status.Complete:
                    case PlanTask.Status.OptionalFailed:
                        // move on to the next task
                        mind.state.plan.TryDequeue(out var result);
                        continue;
                    case PlanTask.Status.Failed:
                        // a task failed
                        continueWithPlan = false;
                        Global.log.trace(
                            $"action plan task {nextTask} failed ({mind.state.plan.Count} following canceled)");
                        mind.state.clearPlan();
                        break;
                }

                if (!continueWithPlan) break;

                switch (nextTask) {
                    case TargetSource nextTarget:
                        // - go to a target
                        navTargetSource = nextTarget;
                        immediateGoal = true; // goal acquired
                        break;
                    case PlanInteraction<DuckMind> inter: {
                        immediateGoal = true; // all interactions are immediate goals
                        processInteraction(inter);

                        break;
                    }
                }
            }

            // constant movement
            if (navTargetSource != null) {
                if (navTargetSource.closeEnoughPosition()) {
                    if (navTargetSource.align) {
                        // TODO: this is an awful cheating hack that skips angle navigation and locks angle instead
                        me.body.angularVelocity = 0f;
                        me.body.stdAngle = navTargetSource.getTargetAngle();
                        // var remaining = pilotToAngle(navTargetSource.getTargetAngle());
                        // if (Math.Abs(remaining) <= TargetSource.AT_ANGLE) {
                        //     // cheat and snap
                        //     me.body.angularVelocity *= 0.8f;
                        // }
                    }
                } else {
                    pilotToPosition(navTargetSource.approachPosition()); // move to position
                }
            } else {
                // zero out movement
                controller.moveThrustLogical.LogicValue = 0f;
                controller.moveTurnLogical.LogicValue = 0f;
            }
        }

        private void processInteraction(PlanInteraction<DuckMind> inter) {
            switch (inter) {
                case PlanFeed interFeed: {
                    // feed a target
                    // TODO: add capability for [HOLD 2s] etc.
                    controller.tetherLogical.LogicPressed = true;
                    interFeed.markDone();

                    break;
                }
                case PlanAttack interAtk: {
                    // attack a target
                    controller.fireLogical.LogicPressed = true;
                    interAtk.markDone();
                    break;
                }
                default:
                    Global.log.err(
                        $"unknown planned interaction {inter.GetType().Name} could not be handled");
                    break;
            }
        }

        /// <summary>
        /// attempts to steer toward position
        /// </summary>
        /// <param name="goal"></param>
        /// <returns>vector to target</returns>
        private Vector2 pilotToPosition(Vector2 goal) {
            // figure out how to move to target
            var toTarget = goal - me.body.pos;
            var targetAngle = toTarget.ScreenSpaceAngle();

            // try to turn to face the right direction
            var remainingTurn = pilotToAngle(targetAngle);

            var thrustInput = 0;

            if (Math.Abs(remainingTurn) < TargetSource.AT_ANGLE) {
                // we are facing the right way
                // now we can move forward

                me.body.angularVelocity *= 0.9f; // cheat to help with angle
                // me.body.angle = -targetAngle + (GMathf.PI / 2);

                var sinPi4 = 0.707106781187; // sin(pi/4), AKA 1/sqrt(2)
                var dGiv = toTarget.Length(); // distance to target
                var v0 = me.body.velocity.Length(); // my velocity
                var vT = me.body.topSpeed / sinPi4; // my top overall speed
                var vTBs = me.body.boostTopSpeed / sinPi4; // my top boost speed
                var aTh = me.body.thrustPower / sinPi4; // my thrust acceleration
                var aBs = (me.body.thrustPower / sinPi4) * me.body.boostFactor; // my boost acceleration
                var aD = me.body.baseDrag / sinPi4; // my linear drag
                var aF = me.body.brakeDrag / sinPi4; // my linear drag when applying brakes
                // calculate d-star
                // we accelerate until this distance
                // TODO: document this calculation
                var dCrit =
                    +(v0 * v0) / (aD + aF)
                    + 0.5 * ((v0 * v0) / (-(aD - aF)));

                // d-star for boost
                // we boost until this distance
                // var dCritBs = v0 * ((vTBs - v0) / (aBs - aD))
                //             + (((vTBs - v0) * (vTBs - v0)) / (2 * (aBs - aD)))
                //             + (vTBs * vTBs) / (aD + aF)
                //             + 0.5 * ((vTBs * vTBs) / (-(aD - aF)));

                var dCritBs = dCrit * 1.1f;

                // update board
                mind.state.setBoard("d_giv", new DuckMindState.BoardItem($"{dGiv:n2}", "mov"));
                mind.state.setBoard("d_crit", new DuckMindState.BoardItem($"{dCrit:n2}", "mov"));

                if (dGiv > dCrit) {
                    thrustInput = -1; // UP on thrust, accelerate

                    // boosting
                    // if (dGiv > dCritBs) {
                    //     controller.boostLogical.logicPressed = true;
                    // }
                } else {
                    thrustInput = 1; // decelerate
                }
            }

            controller.moveThrustLogical.LogicValue = thrustInput;

            return toTarget;
        }

        /// <summary>
        /// tries to pilot to the target angle
        /// </summary>
        /// <param name="targetAngle"></param>
        /// <returns>the remaining angle</returns>
        private float pilotToAngle(float targetAngle) {
            // delta between current angle to target
            var remainingAngle = Mathf.DeltaAngleRadians(me.body.stdAngle, targetAngle);
            Global.log.trace($"rem: {remainingAngle}, tgt: {targetAngle}, cur: {me.body.stdAngle}");
            if (Math.Abs(remainingAngle) > TargetSource.AT_ANGLE) {
                var turnInput = 0f;
                var turnPower = 1f;

                if (remainingAngle > 0) {
                    turnInput = -1 * turnPower;
                } else if (remainingAngle < 0) {
                    turnInput = 1 * turnPower;
                }

                controller.moveTurnLogical.LogicValue = turnInput;
            } else {
                // cheat and snap angle
                me.body.stdAngle = targetAngle;
            }

            return remainingAngle;
        }
    }
}