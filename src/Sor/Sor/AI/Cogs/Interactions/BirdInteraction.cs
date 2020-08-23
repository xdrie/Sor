using Ducia.Cogs.Social;

namespace Sor.AI.Cogs.Interactions {
    public abstract class BirdInteraction : Interaction<DuckMind> {
        public override void run(params DuckMind[] participants) {
            var me = participants[0];
            if (participants.Length == 2) {
                runTwo(me, participants[1]);
            }
        }

        public virtual void runTwo(DuckMind me, DuckMind them) { }
    }
}