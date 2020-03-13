using System;
using XNez.GUtils.Misc;

namespace Sor.AI.Cogs.Interactions {
    public class NearbyBirdInteraction : BirdInteraction {
        private float dist;
        public static float nearRange = MindConstants.SENSE_RANGE / 4;

        public NearbyBirdInteraction(float dist) {
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
            
            var maxOpinionDelta = 4;
            var opinionDelta = -maxOpinionDelta * (me.traits.wary + 1);
            var currentOpinion = me.mind.state.getOpinion(them.mind);
            if (currentOpinion > MindConstants.OPINION_ALLY) {
                opinionDelta = Math.Abs(opinionDelta);
            } else {
                // TODO: take anxiety better into account
                me.emotions.fear = 1; // overwrite fear
            }
            
            opinionDelta = GMathf.clamp(opinionDelta, -maxOpinionDelta, maxOpinionDelta);
            var newOpinion = me.mind.state.addOpinion(them.mind, GMathf.roundToInt(opinionDelta));
        }
    }
}