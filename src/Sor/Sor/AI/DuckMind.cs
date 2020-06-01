using Ducia.Layer1;
using Sor.AI.Cogs;

namespace Sor.AI {
    public class DuckMind : Mind<DuckMindState> {
        public readonly AvianSoul soul;

        public DuckMind(AvianSoul soul) : base(new DuckMindState()) {
            this.soul = soul;
        }

        protected override void think() {
            base.think();

            // update soul
            soul.tick();
        }
    }
}