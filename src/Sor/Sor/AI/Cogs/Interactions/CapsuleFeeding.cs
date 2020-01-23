using System;
using LunchtimeGears.Calc;
using Sor.AI.Signals;

namespace Sor.AI.Cogs.Interactions {
    public class CapsuleFeeding : BirdInteraction {
        private ItemSignals.CapsuleAcquiredSignal sig;

        public CapsuleFeeding(ItemSignals.CapsuleAcquiredSignal sig) {
            this.sig = sig;
        }
        
        public override void run(params AvianSoul[] participants) {
            if (participants.Length != 2)
                throw new ArgumentException("only two participants", nameof(participants));
            var giver = participants[0]; // the one who gave me stuff
            var recpt = participants[1]; // this should be "me"
            
            // TODO: actually look at traits
            // for now, blindly increase our opinion
            var baseBonus = (int) (sig.energy / 400f) * 100;
            baseBonus = IntMath.clamp(baseBonus, 0, 100);
            var giverOpi = recpt.mind.state.addOpinion(giver.mind, baseBonus);
        }
    }
}