using System;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Plans;

namespace Sor.AI.Doer {
    public class PlanExecutor {
        private readonly DuckMind mind;

        public PlanExecutor(DuckMind mind) {
            this.mind = mind;
        }

        public void process() {
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
                    case PlanInteraction inter: {
                        immediateGoal = true; // all interactions are immediate goals
                        processInteraction(inter);

                        break;
                    }
                }
            }

            // constant movement
            if (navTargetSource != null) {
                // move to position
                pilotToPosition(navTargetSource.approachPosition());
                if (navTargetSource.align && navTargetSource.closeEnoughPosition()) {
                    pilotToAngle(navTargetSource.getTargetAngle());
                }
            }
        }

        private void processInteraction(PlanInteraction inter) {
            switch (inter) {
                case PlanFeed interFeed: {
                    // feed a target
                    // TODO: add capability for [HOLD 2s] etc.
                    mind.state.controller.tetherLogical.LogicPressed = true;
                    interFeed.markDone();

                    break;
                }
                case PlanAttack interAtk: {
                    // attack a target
                    mind.state.controller.fireLogical.LogicPressed = true;
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
            var toTarget = goal - mind.state.me.body.pos;
            var targetAngle = toTarget.ScreenSpaceAngle();
            var remainingTurn = pilotToAngle(targetAngle);

            var thrustInput = 0;

            if (Math.Abs(remainingTurn) < TargetSource.AT_ANGLE) {
                // we are facing the right way

                mind.state.me.body.angularVelocity *= 0.9f; // cheat to help with angle
                // mind.state.me.body.angle = -targetAngle + (GMathf.PI / 2);

                var sinPi4 = 0.707106781187; // sin(pi/4)
                // we are facing, now move toward them
                var dGiv = toTarget.Length();
                var v0 = mind.state.me.body.velocity.Length();
                var vT = mind.state.me.body.topSpeed / sinPi4;
                var vTBs = mind.state.me.body.boostTopSpeed / sinPi4;
                var aTh = mind.state.me.body.thrustPower / sinPi4;
                var aBs = (mind.state.me.body.thrustPower / sinPi4) * mind.state.me.body.boostFactor;
                var aD = mind.state.me.body.baseDrag / sinPi4;
                var aF = mind.state.me.body.brakeDrag / sinPi4;
                // d-star
                var dCrit =
                    +(v0 * v0) / (aD + aF)
                    + 0.5 * ((v0 * v0) / (-(aD - aF)));

                // var dCritBs = v0 * ((vTBs - v0) / (aBs - aD))
                //             + (((vTBs - v0) * (vTBs - v0)) / (2 * (aBs - aD)))
                //             + (vTBs * vTBs) / (aD + aF)
                //             + 0.5 * ((vTBs * vTBs) / (-(aD - aF)));

                var dCritBs = dCrit * 1.1f;

                // update board
                mind.state.setBoard(nameof(dGiv), new DuckMindState.BoardItem($"{dGiv:n2}", "mov"));
                mind.state.setBoard(nameof(dCrit), new DuckMindState.BoardItem($"{dCrit:n2}", "mov"));

                if (dGiv > dCrit) {
                    thrustInput = -1;
                    // if (dGiv > dCritBs) {
                    //     controller.boostLogical.logicPressed = true;
                    // }
                }
                else {
                    thrustInput = 1;
                }
            }

            mind.state.controller.moveThrustLogical.LogicValue = thrustInput;

            return toTarget;
        }

        private float pilotToAngle(float targetAngle) {
            // delta between current angle to target
            var remainingAngle = Mathf.DeltaAngleRadians(mind.state.me.body.stdAngle, targetAngle);
            if (Math.Abs(remainingAngle) > TargetSource.AT_ANGLE) {
                var turnInput = 0;

                if (remainingAngle > 0) {
                    turnInput = -1;
                }
                else if (remainingAngle < 0) {
                    turnInput = 1;
                }

                mind.state.controller.moveTurnLogical.LogicValue = turnInput;
            }
            else {
                // cheat and snap angle
                // mind.state.me.body.stdAngle = targetAngle;
            }

            return remainingAngle;
        }
    }
}