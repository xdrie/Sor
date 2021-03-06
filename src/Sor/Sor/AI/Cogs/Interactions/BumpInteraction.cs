using Ducia.Calc;
using Ducia.Cogs;
using Sor.AI.Signals;

namespace Sor.AI.Cogs.Interactions {
    public class BumpInteraction : BirdInteraction {
        private PhysicalSignals.BumpSignal sig;
        
        struct Traits {
            public static float[] vec_annoyed = {0.6f, 0.3f};
            public float annoyed;

            public Traits(AvianSoul soul) {
                var tr = soul.traits;
                annoyed = VectorTrait.trait2(vec_annoyed, new[] {
                    tr.wary, tr.spontaneity
                });
            }
        }

        public BumpInteraction(PhysicalSignals.BumpSignal sig) {
            this.sig = sig;
        }

        public override void runTwo(DuckMind me, DuckMind them) {
            var myTraits = new Traits(me.soul);

            var opinionDelta = 0;
            
            // being bumped annoys me
            var bumpedAnnoyance = (int) TraitCalc.transform(-myTraits.annoyed,
                -20f, 0f);
            // fear exacerbates anger/annoyance
            var fearMultiplier = TraitCalc.transform(me.soul.emotions.fear,
                -1f, 3f, 0f, 2f);

            opinionDelta += (int) (bumpedAnnoyance * fearMultiplier);
            
            me.state.addOpinion(them.state.me, opinionDelta);
            me.soul.emotions.spikeFear(0.4f); // somewhat scary
        }
    }
}