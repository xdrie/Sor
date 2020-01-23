using LunchtimeGears.Units;

namespace Sor.AI.Cogs {
    public class AvianSoul : Sentient {
        public BirdPersonality ply;
        public BirdTraits traits;
        public BirdEmotions emotions;
        public Mind mind;

        public AvianSoul() {
            emotions = new BirdEmotions();
        }
        
        public static AvianSoul generate(Mind mind) {
            var soul = new AvianSoul();
            soul.mind = mind;
            // create personality
            soul.ply = BirdPersonality.makeRandom();
            return soul;
        }

        public void calculateTraits() {
            traits = new BirdTraits(ply);
        }
    }
}