using System;
using Glint.AI.Misc;

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
            
            var maxDel = 4;
            var opDel = -maxDel * (me.traits.wary + 1);
            var currentOp = me.mind.state.getOpinion(them.mind);
            if (currentOp > MindConstants.OPINION_ALLY) {
                opDel = Math.Abs(opDel);
            } else {
                me.emotions.fear = 1; // overwrite fear
            }
            
            opDel = Gmathf.clamp(opDel, -maxDel, maxDel);
            var newOpi = me.mind.state.addOpinion(them.mind, Gmathf.roundToInt(opDel));
        }
    }
}