using System;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Plans;
using XNez.GUtils.Misc;

namespace Sor.AI.Doer {
    public class PlanExecutor {
        private readonly Mind mind;

        public PlanExecutor(Mind mind) {
            this.mind = mind;
        }

        public void process() {
            var targetPosition = default(Vector2?);
            var immediateGoal = false; // whether we've obtained an immediate goal
            while (mind.state.plan.Count > 0 && !immediateGoal) { // go through goals until we find one to execute
                mind.state.plan.TryPeek(out var nextTask);
                // remove invalid goals
                if (!nextTask.valid()) {
                    mind.state.plan.TryDequeue(out var result);
                    continue;
                }

                switch (nextTask) {
                    case TargetSource nextTarget:
                        // - go to a target
                        // check closeness
                        if (nextTarget.closeEnoughApproach(mind.me.body.pos)) {
                            mind.state.plan.TryDequeue(out var result);
                            continue;
                        }

                        targetPosition = nextTarget.approachPosition(mind.me.body.pos);
                        immediateGoal = true; // goal acquired
                        break;
                    case PlanInteraction inter: {
                        immediateGoal = true; // all interactions are immediate goals
                        processInteraction(inter);
                        mind.state.plan.TryDequeue(out var result); // dequeue handled interaction

                        break;
                    }
                }
            }

            // constant movement
            if (targetPosition.HasValue) {
                pilotToPosition(targetPosition.Value);
            }
        }

        private void processInteraction(PlanInteraction inter) {
            switch (inter) {
                case PlanFeed interFeed: {
                    // ensure alignment
                    // TODO: follow target should better try to align
                    var dirToOther = interFeed.target.Position - mind.me.body.pos;
                    dirToOther.Normalize();
                    // get facing dir
                    var facingDir = new Vector2(GMathf.cos(mind.me.body.stdAngle),
                        -GMathf.sin(mind.me.body.stdAngle));
                    if (dirToOther.dot(facingDir) > 0.6f) { // make sure facing properly
                        // feed
                        mind.controller.tetherLogical.logicPressed = true;
                    }

                    break;
                }
                case PlanAttack interAtk: {
                    // TODO: attempt to attack

                    break;
                }
            }
        }

        private void pilotToPosition(Vector2 goal) {
            // figure out how to move to target
            var toTarget = goal - mind.me.body.pos;
            var targetAngle = -GMathf.atan2(toTarget.Y, toTarget.X);
            var myAngle = mind.me.body.stdAngle;
            var turnTo = Mathf.DeltaAngleRadians(myAngle, targetAngle);

            var moveX = 0;
            var moveY = 0;

            if (Math.Abs(turnTo) < 0.05f * GMathf.PI) {
                mind.me.body.angularVelocity *= 0.9f;
                // mind.me.body.angle = -targetAngle + (GMathf.PI / 2);

                var sinPi4 = 0.707106781187; // sin(pi/4)
                // we are facing, now move toward them
                var dGiv = toTarget.Length();
                var v0 = mind.me.body.velocity.Length();
                var vT = mind.me.body.topSpeed / sinPi4;
                var vTBs = mind.me.body.boostTopSpeed / sinPi4;
                var aTh = mind.me.body.thrustPower / sinPi4;
                var aBs = (mind.me.body.thrustPower / sinPi4) * mind.me.body.boostFactor;
                var aD = mind.me.body.baseDrag / sinPi4;
                var aF = mind.me.body.brakeDrag / sinPi4;
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
                mind.state.setBoard(nameof(dGiv), new MindState.BoardItem($"{dGiv:n2}", "mov"));
                mind.state.setBoard(nameof(dCrit), new MindState.BoardItem($"{dCrit:n2}", "mov"));

                if (dGiv > dCrit) {
                    moveY = -1;
                    // if (dGiv > dCritBs) {
                    //     controller.boostLogical.logicPressed = true;
                    // }
                } else {
                    moveY = 1;
                }
            } else {
                if (turnTo > 0) {
                    moveX = -1;
                } else if (turnTo < 0) {
                    moveX = 1;
                }
            }

            mind.controller.moveDirectionLogical.LogicValue = new Vector2(moveX, moveY);
        }
    }
}