using Ducia.Cogs;

namespace Sor.AI.Cogs {
    public class AvianSoul : Sentient<BirdPersonality, BirdTraits, BirdEmotions> {
        public DuckMind mind { get; set; }

        public AvianSoul() { }
    }
}