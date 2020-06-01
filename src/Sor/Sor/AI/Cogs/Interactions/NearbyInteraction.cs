using System;
using Ducia.Calc;
using Microsoft.Xna.Framework;
using Nez;
using XNez.GUtils.Misc;

namespace Sor.AI.Cogs.Interactions {
    public class NearbyInteraction : BirdInteraction {
        private float dist;
        public const float triggerRange = Constants.DuckMind.SENSE_RANGE / 4;
        private const float closeDistance = 40f;

        public NearbyInteraction(float dist) {
            this.dist = dist;
        }

        public override void run(params DuckMind[] participants) {
            var me = participants[0];
            var them = participants[1];

            // when a bird is nearby
            // if opinion is negative, then there is cause for fear
            // if opinion is positive, then i feel secure
            // if my anxiety is high, i will react more strongly to both

            // if you get WAY too close, you will be labeled a threat

            var maxOpinionDelta = 4; // range [-4, 4]
            var opinionDelta = 0;
            var currentOpinion = me.state.getOpinion(them.state.me);
            if (currentOpinion > Constants.DuckMind.OPINION_ALLY) {
                // we're friends, we should have a positive opinion
                // this is based on both anxiety and sociability.
                // TODO: positive opinion from being near friends
                // this should fall off to a negligible account above a certain threshold
            } else {
                // TODO: take anxiety better into account
                // calculate opinion-affecting factors
                var maxWariness = 10f;
                // [-4, 0]: long-distance wariness
                var longDistanceWariness =
                    (int) TraitCalc.transform(-me.soul.traits.wary, -4, 2, -4, 0);
                // [-4, -1]: close distance wariness
                var closeWariness = 0;
                if (dist < closeDistance) {
                    // extreme caution
                    closeWariness =
                        (int) TraitCalc.transform(-me.soul.traits.wary, -4, 0, -4, -1);
                }

                me.state.setBoard("nearby fear",
                    new DuckMindState.BoardItem($"L: {longDistanceWariness}, C: {closeWariness}", "interaction",
                        Color.Orange, Time.TotalTime + 1f));
                var warinessScore = longDistanceWariness + closeWariness;
                opinionDelta += warinessScore;
                
                // being in the presence of a threat is scary
                me.soul.emotions.spikeFear(Math.Abs(warinessScore / maxWariness));
            }

            // clamp the opinion delta to the required range
            opinionDelta = GMathf.clamp(opinionDelta, -maxOpinionDelta, maxOpinionDelta);
            me.state.addOpinion(them.state.me, GMathf.roundToInt(opinionDelta));
        }
    }
}