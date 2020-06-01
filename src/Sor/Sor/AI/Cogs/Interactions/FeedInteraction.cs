using System;
using DuckMind.Calc;
using DuckMind.Cogs;
using Sor.AI.Signals;
using XNez.GUtils.Misc;

namespace Sor.AI.Cogs.Interactions {
    public class FeedInteraction : BirdInteraction {
        private ItemSignals.CapsuleAcquiredSignal sig;

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

        public FeedInteraction(ItemSignals.CapsuleAcquiredSignal sig) {
            this.sig = sig;
        }

        public override void run(params AvianSoul[] participants) {
            if (participants.Length != 2)
                throw new ArgumentException("only two participants", nameof(participants));
            var me = participants[0]; // this should be "me"
            var myTraits = new Traits(me);
            var giver = participants[1]; // the one who gave me stuff

            // food value [0, 40]
            var maxFoodValue = 40;
            var foodValue = (int) (sig.energy / 400f) * maxFoodValue;

            // calculate opinion delta
            var opinionDelta = 0;
            var currentOpinion = me.mind.state.getOpinion(giver.mind);

            // calculate receptiveness to food
            // receptive (innate) [0, 1]
            var innateFoodReceiptiveness = TraitCalc.transform(myTraits.receptiveness,
                -0.4f, 1f, 0f, 1f);
            // receptive (happy) [0, 0.5]
            var happyFoodReceptiveness = TraitCalc.transform(me.emotions.happy,
                -1.5f, 1f, 0f, 0.5f);

            // receptive [0, 2]
            var foodReceptiveness = GMathf.clamp(
                innateFoodReceiptiveness + happyFoodReceptiveness,
                0f, 2f
            );

            // significantly diminishing rewards, effectively capping around ~300
            var foodOpinionWeight = LCurves.diminishingReturns(currentOpinion / 10f, 1f, 0.1f);
            opinionDelta += (int) (foodReceptiveness * foodValue * foodOpinionWeight);

            // food makes me happy!
            me.emotions.spikeHappy(GMathf.clamp(foodReceptiveness, 0, 0.8f));

            // add opinion to the one that fed me
            me.mind.state.addOpinion(giver.mind, opinionDelta);
        }
    }
}