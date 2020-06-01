using Ducia.Layer1;
using Sor.AI.Cogs;

namespace Sor.AI {
    public class DuckMind : Mind<DuckMindState> {
        public bool control;
        public readonly AvianSoul soul;

        public DuckMind(AvianSoul soul, bool control) : base(new DuckMindState()) {
            this.control = control;
            this.soul = soul;
        }

        protected override void think() {
            base.think();

            // update soul
            soul.tick();
        }
    }
}