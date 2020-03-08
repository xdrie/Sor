using LunchLib.Cogs;

namespace Sor.AI.Cogs {
    public class BirdTraits : Traits<BirdPersonality> {
        public static float[] vec_loyalty = {-0.3f, 0.8f};
        public static float[] vec_aggression = {0.6f, -0.5f};
        public static float[] vec_wary = {0.9f, -0.8f};
        public static float[] vec_inquisitive = {-0.3f, 0.1f};
        public static float[] vec_sociability = {-0.1f, 1.0f};

        public float loyalty;
        public float aggression;
        public float sociability;
        public float wary;
        public float inquisitive;

        public override float[] vec => new [] { loyalty, aggression, sociability, wary, inquisitive };

        public BirdTraits(BirdPersonality ply) : base(ply) {
            loyalty = VectorTrait.value(vec_loyalty, ply);
            aggression = VectorTrait.value(vec_aggression, ply);
            wary = VectorTrait.value(vec_wary, ply);
            sociability = VectorTrait.value(vec_sociability, ply);
            inquisitive = VectorTrait.value(vec_inquisitive, ply);
        }
    }
}