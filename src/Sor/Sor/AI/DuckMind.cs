using Ducia;
using Sor.AI.Cogs;
using Sor.AI.Doer;
using Sor.AI.Systems;
using Sor.Components.Input;
using Sor.Components.Units;

namespace Sor.AI {
    public class DuckMind : Mind<DuckMindState> {
        public bool control;
        public readonly AvianSoul soul;

        private PlanExecutor planExecutor;

        public DuckMind(AvianSoul soul, bool control) : base(new DuckMindState()) {
            this.control = control;
            this.soul = soul;
            useThreadPool = NGame.config.threadPoolAi;
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            
            // load components
            state.me = Entity.GetComponent<Wing>();
            state.controller = Entity.GetComponent<LogicInputController>();

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
            if (control) {
                base.act();
            }

            planExecutor.process();
        }
    }
}