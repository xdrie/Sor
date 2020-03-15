using LunchLib.Cogs;
using LunchLib.Cogs.Social;

namespace Sor.AI.Cogs.Interactions {
    public abstract class BirdInteraction : Interaction<AvianSoul, BirdPersonality, BirdTraits, BirdEmotions> {
        public override void run(params AvianSoul[] participants) {
            var me = participants[0];
            if (participants.Length == 2) {
                runTwo(me, participants[1]);
            }
        }

        public virtual void runTwo(AvianSoul me, AvianSoul them) { }
    }
}