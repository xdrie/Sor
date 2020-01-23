using Glint.AI.Misc;
using LunchtimeGears.Calc;

namespace Sor.AI.Cogs {
    public struct BirdPersonality {
        public float A; // anxiety
        public float S; // extraversion

        // full names
        public float anxiety => A;
        public float social => S;

        public float v0;
        public float v1;

        public float[] disposition => new[] {v0, v1};

        // -- calculation
        // normalize personality component from [-1,1] to [0,1]

        // inverse normalize from [0,1] to [-1,1]
        public float inm(float v) => (v * 2) - 1;

        public float[] vec => Mathf.normVec(new[] {A, S});

        public float mult(float[] weights) {
            return Mathf.dot(vec, weights);
        }

        public void generate() {
            // TODO: make this a more interesting curve
            var vfU = 0f;
            var vfS = 0.4f;
            S = Mathf.clamp(Distribution.normalRand(vfU, vfS), -1f, 1f);
            A = Mathf.clamp(Distribution.normalRand(vfU, vfS), -1f, 1f);
            // generate humor vector
            // TODO: this should be weighted based on other traits 
            // so that similar personalities tend toward similar humor
            var diVfU = 0f;
            var diVfS = 0.6f;
            v0 = Mathf.clamp(Distribution.normalRand(diVfU, diVfS), -1f, 1f);
            v1 = Mathf.clamp(Distribution.normalRand(diVfU, diVfS), -1f, 1f);
        }
    }
}