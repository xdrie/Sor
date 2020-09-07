using Ducia;
using Ducia.Game.Mind;
using Nez;
using Sor.AI.Cogs;
using Sor.AI.Doer;
using Sor.AI.Systems;
using Sor.Components.Input;
using Sor.Components.Units;

namespace Sor.AI {
    public class DuckMind : Mind<DuckMindState> {
        public bool control;
        public readonly AvianSoul soul;
        public bool inspected = false;

        private PlanExecutor planExecutor;

        public Wing me => state.me;
        public Entity entity => me.Entity;

        public DuckMind(AvianSoul soul, bool control) : base(new DuckMindState()) {
            this.control = control;
            this.soul = soul;
            useThreadPool = NGame.config.threadPoolAi;
        }

        /// <summary>
        /// attach this mind to a wing's entity
        /// </summary>
        /// <param name="wing"></param>
        public void attach(Wing wing) {
            var nt = wing.Entity;

            nt.AddComponent(new MindComponent(this));

            // load components
            state.me = nt.GetComponent<Wing>();
            state.controller = nt.GetComponent<LogicInputController>();
        }

        public override void initialize() {
            base.initialize();
            
            // set up plan executor
            planExecutor = new PlanExecutor(this);

            // set up systems
            sensorySystems.Add(new VisionSystem(this, 0.4f, cancelToken.Token));
            cognitiveSystems.Add(new ThinkSystem(this, 0.4f, cancelToken.Token));
        }

        protected override void think() {
            if (control) {
                base.think();
            }

            // update soul
            soul.tick();
        }

        protected override void act() {
            if (!control) return;
            base.act();
            planExecutor.process();
        }
    }
}