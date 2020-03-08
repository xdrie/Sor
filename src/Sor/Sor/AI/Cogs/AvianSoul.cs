using LunchLib.Cogs;

namespace Sor.AI.Cogs {
    public class AvianSoul : Sentient<BirdPersonality, BirdTraits, BirdEmotions> {
        public Mind mind;

        public AvianSoul() { }

        public AvianSoul(Mind mind) {
            this.mind = mind;
        }
    }
}