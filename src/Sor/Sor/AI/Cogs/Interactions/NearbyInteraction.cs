using LunchLib.Calc;
using XNez.GUtils.Misc;

namespace Sor.AI.Cogs.Interactions {
    public class NearbyInteraction : BirdInteraction {
        private float dist;
        public static float nearRange = MindConstants.SENSE_RANGE / 4;

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
                // [-4, 0]: long-distance wariness
                var longDistanceWariness =
                    (int) GMathExt.transformTrait(me.traits.wary, -4, 2, -4, 0);
                // [-4, 0]: short distance wariness
                var shortDistanceWariness = 0;

                opinionDelta = longDistanceWariness + shortDistanceWariness;
                // being in the presence of a threat is scary
                me.emotions.fear = 1;
            }

            // clamp the opinion delta to the required range
            opinionDelta = GMathf.clamp(opinionDelta, -maxOpinionDelta, maxOpinionDelta);
            me.mind.state.addOpinion(them.mind, GMathf.roundToInt(opinionDelta));
        }
    }
}