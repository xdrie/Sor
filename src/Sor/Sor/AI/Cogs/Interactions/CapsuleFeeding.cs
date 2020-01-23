using System;

namespace Sor.AI.Cogs.Interactions {
    public class CapsuleFeeding : BirdInteraction {
        public override void run(params AvianSoul[] participants) {
            if (participants.Length != 2)
                throw new ArgumentException("only two participants", nameof(participants));
            var giver = participants[0]; // the one who gave me stuff
            var recpt = participants[1]; // this should be "me"
            
            // TODO: actually look at traits
            // for now, blindly increase our opinion
            var bonus = 10;
            var giverOpi = recpt.mind.state.addOpinion(giver.mind, bonus);
        }
    }
}