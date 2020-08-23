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

        public override float[] vec => new[] {loyalty, aggression, sociability, wary, inquisitive};

        public override void calculate(BirdPersonality ply) {
            loyalty = VectorTrait.value(vec_loyalty, ply);
            aggression = VectorTrait.value(vec_aggression, ply);
            wary = VectorTrait.value(vec_wary, ply);
            sociability = VectorTrait.value(vec_sociability, ply);
            inquisitive = VectorTrait.value(vec_inquisitive, ply);
            spontaneity = VectorTrait.value(vec_spontaneity, ply);
        }
    }
}