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

        public AvianSoul(BirdPersonality ply) : this() {
            this.ply = ply;
        }
        
        public static AvianSoul generate(Mind mind) {
            var soul = new AvianSoul();
            soul.mind = mind;
            // create personality
            soul.ply = BirdPersonality.makeRandom();
            return soul;
        }

        public void calc() {
            traits = new BirdTraits(ply);
        }

        public void tick() {
            emotions.tick();
        }
    }
}