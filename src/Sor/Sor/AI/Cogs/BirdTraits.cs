using LunchtimeGears.Cogs;

namespace Sor.AI.Cogs {
    public class BirdTraits {
        public static float[] vec_loyalty = {-0.3f, 0.8f};
        public static float[] vec_aggression = {0.6f, -0.5f};
        public static float[] vec_fear = {0.9f, -0.8f};

        public float loyalty;
        public float aggression;
        public float fear;

        public BirdTraits(BirdPersonality ply) {
            loyalty = VectorTrait.value(vec_loyalty, ply);
            aggression = VectorTrait.value(vec_aggression, ply);
            fear = VectorTrait.value(vec_fear, ply);
        }
    }
}