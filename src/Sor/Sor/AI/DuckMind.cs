using Ducia.Layer1;
using Sor.AI.Cogs;
using Sor.AI.Systems;
using Sor.Components.Input;
using Sor.Components.Units;

namespace Sor.AI {
    public class DuckMind : Mind<DuckMindState> {
        public bool control;
        public readonly AvianSoul soul;

        public DuckMind(AvianSoul soul, bool control) : base(new DuckMindState()) {
            this.control = control;
            this.soul = soul;
            
            // set up systems
            sensorySystems.Add(new VisionSystem(this, 0.4f, cancelToken.Token));
            cognitiveSystems.Add(new ThinkSystem(this, 0.4f, cancelToken.Token));
        }

        public override void Initialize() {
            base.Initialize();

            // load components
            state.me = Entity.GetComponent<Wing>();
            state.controller = Entity.GetComponent<LogicInputController>();
        }

        protected override void think() {
            base.think();

            // update soul
            soul.tick();
        }
    }
}