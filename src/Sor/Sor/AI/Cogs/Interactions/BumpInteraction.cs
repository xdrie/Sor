using LunchLib.Calc;
using LunchLib.Cogs;
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

        public override void runTwo(AvianSoul me, AvianSoul them) {
            var myTraits = new Traits(me);

            var opinionDelta = 0;
            
            // being bumped annoys me
            var beingShotAnger = (int) TraitCalc.transform(-myTraits.annoyed,
                -20f, 0f);

            opinionDelta += beingShotAnger;
            
            me.mind.state.addOpinion(them.mind, opinionDelta);
            me.emotions.spikeFear(0.4f); // somewhat scary
        }
    }
}