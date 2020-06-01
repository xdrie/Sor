using DuckMind.Cogs;

namespace Sor.AI.Cogs {
    public class AvianSoul : Sentient<BirdPersonality, BirdTraits, BirdEmotions> {
        public Mind mind { get; set; }

        public AvianSoul() { }
    }
}