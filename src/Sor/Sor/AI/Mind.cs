using System;
using System.Threading;
using System.Threading.Tasks;
using Glint;
using Glint.Util;
using Nez;
using Sor.AI.Cogs;
using Sor.AI.Doer;
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
        public bool inspected = false;
        public GameContext gameCtx;

        public int consciousnessSleep = 100;
        private Task consciousnessTask;
        private CancellationTokenSource conciousnessCancel;
        private PlanExecutor planExecutor;

        public Mind() : this(null, true) { }

        public Mind(AvianSoul soul, bool control) {
            if (soul == null) {
                // generate a new soul
                this.soul = new AvianSoul(this);
                this.soul.ply.generateRandom(); // randomize its personality
                Global.log.writeLine($"generated soul with personality {this.soul.ply}", GlintLogger.LogLevel.Trace);
            }

            this.soul = soul;
            this.soul.mind = this;
            // run calc on the soul
            this.soul.recalculate();
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
                if (NGame.context.config.threadPoolAi) {
                    consciousnessTask = Task.Run(async () => await consciousnessAsync(conciousnessCancel.Token));
                }

                // set up plan executor
                planExecutor = new PlanExecutor(this);
            }
        }

        public async Task consciousnessAsync(CancellationToken tok) {
            while (!tok.IsCancellationRequested) {
                think();

                await Task.Delay(consciousnessSleep, tok);
            }
        }

        public void Update() {
            // Sense-Think-Act AI
            if (control) {
                sense(); // sense the world around
                act(); // carry out decisions();
                if (!NGame.context.config.threadPoolAi && consciousnessTask == null) {
                    var msPassed = (int) (Time.DeltaTime * 1000);
                    var thinkModulus = consciousnessSleep / msPassed;
                    if (Time.FrameCount % thinkModulus == 0) {
                        // synchronous think
                        think();
                    }
                }
            }

            // update state information
            state.tick();
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
            planExecutor.process();
        }

        private void think() {
            // think based on information and make plans
            try {
                thinkSystem.tick();
            }
            catch (Exception e) {
                // log exceptions in think
                Global.log.writeLine(e.ToString(), GlintLogger.LogLevel.Error);
            }
        }

        private void sense() {
            visionSystem.tick(); // vision
        }

        public void signal(MindSignal signal) {
            state.signalQueue.Enqueue(signal);
        }
    }
}