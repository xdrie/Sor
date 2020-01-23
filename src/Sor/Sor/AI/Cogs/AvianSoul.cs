using LunchtimeGears.Cogs;
using LunchtimeGears.Units;

namespace Sor.AI.Cogs {
    public class AvianSoul : Sentient {
        public BirdPersonality ply;
        
        public AvianSoul generate() {
            // create personality
            ply = new BirdPersonality();
            ply.generate();
            
            return this;
        }
    }
}