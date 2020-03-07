using LunchLib.Cogs;

namespace Sor.AI.Cogs {
    public class BirdEmotions : Emotions {
        // emotions: [fear, happy]
        public override float[] vec { get; } = {0f, 0f};
        public override float falloff => 0.9f;

        public float fear {
            get => vec[0];
            set => vec[0] = value;
        }

        public float happy {
            get => vec[1];
            set => vec[1] = value;
        }
    }
}