using LunchtimeGears.Units;

namespace Sor.AI.Cogs {
    public class AvianSoul : Sentient {
        public BirdPersonality ply;
        public Mind mind;
        
        public static AvianSoul generate(Mind mind) {
            var soul = new AvianSoul();
            soul.mind = mind;
            // create personality
            soul.ply = BirdPersonality.makeRandom();
            return soul;
        }
    }
}