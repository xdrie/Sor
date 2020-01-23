using System;
using LunchtimeGears.Calc;
using LunchtimeGears.Cogs;
using Sor.AI.Signals;

namespace Sor.AI.Cogs.Interactions {
    public class CapsuleFeedingInteraction : BirdInteraction {
        private ItemSignals.CapsuleAcquiredSignal sig;
        private const int maxBonus = 40;

        struct Traits {
            public static float[] vec_receptiveness = {-0.6f, 0.5f};
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
            var giver = participants[0]; // the one who gave me stuff
            var giverTraits = new Traits(giver);
            var recpt = participants[1]; // this should be "me"
            var recptTraits = new Traits(recpt);

            // TODO: actually look at traits
            // for now, blindly increase our opinion
            var baseBonus = (int) (sig.energy / 400f) * maxBonus;
            baseBonus = IntMath.clamp(baseBonus, 0, maxBonus);
            var giverOpi = recpt.mind.state.addOpinion(giver.mind, baseBonus);
        }
    }
}