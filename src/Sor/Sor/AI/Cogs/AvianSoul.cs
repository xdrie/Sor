using LunchtimeGears.Units;

namespace Sor.AI.Cogs {
    public class AvianSoul : Sentient {
        public BirdPersonality ply;
        
        public static AvianSoul generate() {
            var soul = new AvianSoul();
            // create personality
            soul.ply = BirdPersonality.makeRandom();
            return soul;
        }
    }
}