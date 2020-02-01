using System;
using System.Threading;
using System.Threading.Tasks;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Cogs;
using Sor.AI.Signals;
using Sor.AI.Systems;
using Sor.Components.Input;
using Sor.Components.Units;

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

        public int consciousnessSleep = 100;
        protected Task consciousnessTask;
        protected CancellationTokenSource conciousnessCancel;

        public Mind() : this(null, true) { }

        public Mind(AvianSoul inSoul, bool control) {
            soul = inSoul;
            if (soul == null) { // generate soul
                soul = AvianSoul.generate(this);
                soul.calc();
                Global.log.writeLine($"generated soul {soul.ply}", GlintLogger.LogLevel.Trace);
            }

            soul.mind = this;

            this.control = control;
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
                visionSystem = new VisionSystem(this, 0.2f, cts.Token);

                thinkSystem = new ThinkSystem(this, 0.2f, cts.Token);

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

            // figure out how to move to target
            var toTarget = state.target - me.body.pos;
            var targetAngle = -Mathf.Atan2(toTarget.Y, toTarget.X);
            var myAngle = -me.body.angle + (Mathf.PI / 2);
            var turnTo = Mathf.DeltaAngleRadians(myAngle, targetAngle);

            var moveX = 0;
            var moveY = 0;

            if (Math.Abs(turnTo) < 0.05f * Mathf.PI) {
                me.body.angularVelocity *= 0.9f;
                // me.body.angle = -targetAngle + (Mathf.PI / 2);

                // we are facing, now move toward them
                var dGiv = toTarget.Length();
                var v0 = me.body.velocity.Length();
                var vT = me.body.maxVelocity.Length();
                var aTh = me.body.thrustPower;
                var aD = me.body.drag.Length();
                var aF = me.body.flapDrag;
                // d-star
                var dCrit = v0 * ((vT - v0) / (aTh - aD))
                            + (((vT - v0) * (vT - v0)) / (2 * (aTh - aD)))
                            + (vT * vT) / (aD + aF)
                            + 0.5 * ((vT * vT) / (-(aD - aF)));
                // phase 1 t
                var tp1 = (vT - v0) / (aTh - aD);
                var tp2 = (dGiv - dCrit) / vT;
                var tp3 = vT / (aD - aF);
                lock (state.board) {
                    state.board[nameof(tp1)] = new MindState.BoardItem($"{tp1:n2}");
                    state.board[nameof(tp2)] = new MindState.BoardItem($"{tp2:n2}");
                    state.board[nameof(tp3)] = new MindState.BoardItem($"{tp3:n2}");
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