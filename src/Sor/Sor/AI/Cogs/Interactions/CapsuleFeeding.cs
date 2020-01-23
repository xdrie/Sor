using System;

namespace Sor.AI.Cogs.Interactions {
    public class CapsuleFeeding : BirdInteraction {
        public override void run(params AvianSoul[] participants) {
            if (participants.Length != 2)
                throw new ArgumentException("only two participants", nameof(participants));
            var giver = participants[0];
            var recpt = participants[1];
        }
    }
}