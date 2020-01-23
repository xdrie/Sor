using System;
using Glint.AI.Misc;
using LunchtimeGears.Calc;
using LunchtimeGears.Cogs;
using Sor.AI.Signals;

namespace Sor.AI.Cogs.Interactions {
    public class CapsuleFeedingInteraction : BirdInteraction {
        private ItemSignals.CapsuleAcquiredSignal sig;
        private const int maxBonus = 40;

        struct Traits {
            public static float[] vec_receptiveness = {0.9f, -0.4f};
            public float receptiveness;

            public Traits(AvianSoul soul) {
                var tr = soul.traits;
                receptiveness = VectorTrait.trait2(vec_receptiveness, new[] {
                    tr.loyalty, tr.aggression
                });
            }
        }

        public CapsuleFeedingInteraction(ItemSignals.CapsuleAcquiredSignal sig) {
            this.sig = sig;
        }

        public override void run(params AvianSoul[] participants) {
            if (participants.Length != 2)
                throw new ArgumentException("only two participants", nameof(participants));
            var me = participants[0]; // this should be "me"
            var recptTraits = new Traits(me);
            var giver = participants[1]; // the one who gave me stuff
            var giverTraits = new Traits(giver);

            // calculate opinion delta
            var opDel = (int) (sig.energy / 400f) * maxBonus;
            // if receptive, add a bonus
            var opScl = PerMath.twoSegment(opDel, recptTraits.receptiveness, 0f, 1.4f, true);
            opDel = IntMath.clamp((int) (opDel * opScl), 0, maxBonus);
            var giverOpi = me.mind.state.addOpinion(giver.mind, opDel);
        }
    }
}