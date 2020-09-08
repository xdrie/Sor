using Ducia.Cogs;

namespace Sor.AI.Cogs {
    public class BirdTraits : Traits<BirdPersonality> {
        public static float[] vec_loyalty = {-0.5f, 0.8f, 0.4f};
        public static float[] vec_aggression = {0.3f, -0.5f, 0.9f};
        public static float[] vec_wary = {0.9f, -0.8f, 0.4f};
        public static float[] vec_sociability = {-0.1f, 1.0f, 0.3f};
        public static float[] vec_inquisitive = {-0.3f, 0.1f, 0f};
        public static float[] vec_spontaneity = {0.4f, -0.1f, 0.8f};

        public float loyalty;
        public float aggression;
        public float sociability;
        public float wary;
        public float inquisitive;
        public float spontaneity;

        public override void calculate(BirdPersonality ply) {
            loyalty = VectorTrait.normalizedValue(vec_loyalty, ply);
            aggression = VectorTrait.normalizedValue(vec_aggression, ply);
            wary = VectorTrait.normalizedValue(vec_wary, ply);
            sociability = VectorTrait.normalizedValue(vec_sociability, ply);
            inquisitive = VectorTrait.normalizedValue(vec_inquisitive, ply);
            spontaneity = VectorTrait.normalizedValue(vec_spontaneity, ply);
        }
    }
}