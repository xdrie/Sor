using System;
using LunchLib.Calc;
using Microsoft.Xna.Framework;
using Nez;
using XNez.GUtils.Misc;

namespace Sor.AI.Cogs.Interactions {
    public class NearbyInteraction : BirdInteraction {
        private float dist;
        public const float triggerRange = MindConstants.SENSE_RANGE / 4;
        private const float closeDistance = 40f;

        public NearbyInteraction(float dist) {
            this.dist = dist;
        }

        public override void run(params AvianSoul[] participants) {
            var me = participants[0];
            var them = participants[1];

            // when a bird is nearby
            // if opinion is negative, then there is cause for fear
            // if opinion is positive, then i feel secure
            // if my anxiety is high, i will react more strongly to both

            // if you get WAY too close, you will be labeled a threat

            var maxOpinionDelta = 4; // range [-4, 4]
            var opinionDelta = 0;
            var currentOpinion = me.mind.state.getOpinion(them.mind);
            if (currentOpinion > MindConstants.OPINION_ALLY) {
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
                    (int) TraitCalc.transformTrait(-me.traits.wary, -4, 2, -4, 0);
                // [-4, -1]: close distance wariness
                var closeWariness = 0;
                if (dist < closeDistance) {
                    // extreme caution
                    closeWariness =
                        (int) TraitCalc.transformTrait(-me.traits.wary, -4, 0, -4, -1);
                }

                me.mind.state.setBoard("nearby fear",
                    new MindState.BoardItem($"L: {longDistanceWariness}, C: {closeWariness}", "interaction",
                        Color.Orange, Time.TotalTime + 1f));
                var warinessScore = longDistanceWariness + closeWariness;
                opinionDelta += warinessScore;
                
                // being in the presence of a threat is scary
                me.emotions.fear = Math.Abs(warinessScore / maxWariness);
            }

            // clamp the opinion delta to the required range
            opinionDelta = GMathf.clamp(opinionDelta, -maxOpinionDelta, maxOpinionDelta);
            me.mind.state.addOpinion(them.mind, GMathf.roundToInt(opinionDelta));
        }
    }
}