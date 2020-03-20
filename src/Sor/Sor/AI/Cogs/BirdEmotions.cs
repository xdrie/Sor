using LunchLib.Cogs;

namespace Sor.AI.Cogs {
    public class BirdEmotions : Emotions {
        // emotions: [fear, happy]
        public override float[] vec { get; } = {0f, 0f};
        public override float falloff => 0.9f;

        public ref float fear => ref vec[0];
        public ref float happy => ref vec[1];

        public void spikeFear(float val) => spike(ref fear, val);
        public void spikeHappy(float val) => spike(ref happy, val); 
    }
}