using System;
using System.Threading;
using System.Threading.Tasks;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Cogs;
using Sor.AI.Model;
using Sor.AI.Signals;
using Sor.AI.Systems;
using Sor.Components.Input;
using Sor.Components.Units;
using XNez.GUtils.Misc;

namespace Sor.AI {
    /// <summary>
    /// represents the consciousness of a Wing
    /// </summary>
    public class Mind : Component, IUpdatable {
        public bool control; // whether the mind is in control
        public MindState state;
        public LogicInputController controller;
        public Wing me;
        public VisionSystem visionSystem;
        public ThinkSystem thinkSystem;
        public AvianSoul soul;
        public bool debug = false;
        public GameContext gameCtx;

        public int consciousnessSleep = 100;
        protected Task consciousnessTask;
        protected CancellationTokenSource conciousnessCancel;

        public Mind() : this(null, true) { }

        public Mind(AvianSoul soul, bool control) {
            if (soul == null) { // generate a new soul
                this.soul = new AvianSoul(this);
                this.soul.ply.generateRandom(); // randomize its personality
                Global.log.writeLine($"generated soul with personality {this.soul.ply}", GlintLogger.LogLevel.Trace);
            }
            this.soul = soul;
            this.soul.mind = this;
            this.control = control;

            gameCtx = Core.Services.GetService<GameContext>();
        }

        public override void Initialize() {
            base.Initialize();

            me = Entity.GetComponent<Wing>();
            state = new MindState(this);
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            if (control) {
                // input
                controller = Entity.GetComponent<LogicInputController>();

                // mind systems
                var cts = new CancellationTokenSource();
                conciousnessCancel = cts;

                visionSystem = new VisionSystem(this, 0.4f, cts.Token);
                thinkSystem = new ThinkSystem(this, 0.4f, cts.Token);

                // start processing tasks
                consciousnessTask = Task.Run(async () => await consciousnessAsync(conciousnessCancel.Token));
            }
        }

        public async Task consciousnessAsync(CancellationToken tok) {
            while (!tok.IsCancellationRequested) {
                try {
                    think(); // think based on information and make plans
                }
                catch (Exception e) {
                    // log exceptions in think
                    Global.log.writeLine(e.ToString(), GlintLogger.LogLevel.Error);
                }

                await Task.Delay(consciousnessSleep, tok);
            }
        }

        public void Update() { // Sense-Think-Act AI
            if (control) {
                sense(); // sense the world around
                act(); // carry out decisions
            }
        }

        public override void OnRemovedFromEntity() {
            base.OnRemovedFromEntity();

            if (control) {
                // stop processing tasks
                conciousnessCancel.Cancel();
            }
        }

        private void act() {
            if (control) {
                controller.zero(); // reset the controller
            }

            // execute plan
            var targetPosition = default(Vector2?);
            lock (state.plan) {
                while (state.plan.Count > 0) {
                    // check target validity
                    var nextTask = state.plan.Peek();
                    if (nextTask is TargetSource nextTarget) {
                        if (!nextTarget.valid()) {
                            state.plan.Dequeue(); // it's invalid, remove it
                            continue;
                        }

                        // check closeness
                        if (nextTarget.closeEnoughApproach(me.body.pos)) {
                            state.plan.Dequeue();
                            continue;
                        }

                        targetPosition = nextTarget.approachPosition(me.body.pos);
                        break;
                    } else if (nextTask is PlanInteraction inter) {
                        switch (nextTask) {
                            case PlanFeed interFeed: {
                                if (inter.valid()) {
                                    // ensure alignment
                                    // TODO: follow target should better try to align
                                    var dirToOther = interFeed.feedTarget.Position - me.body.pos;
                                    dirToOther.Normalize();
                                    // get facing dir
                                    var facingDir = new Vector2(GMathf.cos(me.body.stdAngle),
                                        -GMathf.sin(me.body.stdAngle));
                                    if (dirToOther.dot(facingDir) > 0.6f) { // make sure facing properly
                                        // feed
                                        controller.tetherLogical.logicPressed = true;
                                    }
                                }

                                break;
                            }
                        }

                        // now dequeue
                        state.plan.Dequeue();
                    }
                }
            }

            if (targetPosition.HasValue) {
                pilotToPosition(targetPosition.Value);
            }
        }

        private void pilotToPosition(Vector2 goal) {
            // figure out how to move to target
            var toTarget = goal - me.body.pos;
            var targetAngle = -GMathf.atan2(toTarget.Y, toTarget.X);
            var myAngle = me.body.stdAngle;
            var turnTo = Mathf.DeltaAngleRadians(myAngle, targetAngle);

            var moveX = 0;
            var moveY = 0;

            if (Math.Abs(turnTo) < 0.05f * GMathf.PI) {
                me.body.angularVelocity *= 0.9f;
                // me.body.angle = -targetAngle + (GMathf.PI / 2);

                var sinPi4 = 0.707106781187; // sin(pi/4)
                // we are facing, now move toward them
                var dGiv = toTarget.Length();
                var v0 = me.body.velocity.Length();
                var vT = me.body.topSpeed / sinPi4;
                var vTBs = me.body.boostTopSpeed / sinPi4;
                var aTh = me.body.thrustPower / sinPi4;
                var aBs = (me.body.thrustPower / sinPi4) * me.body.boostFactor;
                var aD = me.body.baseDrag / sinPi4;
                var aF = me.body.brakeDrag / sinPi4;
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
                lock (state.board) {
                    state.board[nameof(dGiv)] = new MindState.BoardItem($"{dGiv:n2}", "mov");
                    state.board[nameof(dCrit)] = new MindState.BoardItem($"{dCrit:n2}", "mov");
                }

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

            controller.moveDirectionLogical.LogicValue = new Vector2(moveX, moveY);
        }

        private void think() {
            thinkSystem.tick();
        }

        private void sense() {
            visionSystem.tick(); // vision
        }

        public void signal(MindSignal signal) {
            state.signalQueue.Enqueue(signal);
        }
    }
}