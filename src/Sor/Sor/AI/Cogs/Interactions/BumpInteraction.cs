using Sor.AI.Signals;

namespace Sor.AI.Cogs.Interactions {
    public class BumpInteraction : BirdInteraction {
        private PhysicalSignals.BumpSignal sig;

        public BumpInteraction(PhysicalSignals.BumpSignal sig) {
            this.sig = sig;
        }

        public override void runTwo(AvianSoul me, AvianSoul them) {
            // TODO: more nuanced reaction to getting bumped
            
            // we don't like getting bumped
            me.mind.state.addOpinion(them.mind, -10);
            me.emotions.spikeFear(0.4f); // somewhat scary
        }
    }
}