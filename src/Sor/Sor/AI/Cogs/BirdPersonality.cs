using Glint.AI.Misc;
using LunchtimeGears.Calc;
using LunchtimeGears.Cogs;

namespace Sor.AI.Cogs {
    public class BirdPersonality : Personality {
        public float A; // anxiety
        public float S; // extraversion

        // full names
        public float anxiety => A;
        public float social => S;

        public float v0;
        public float v1;

        public float[] disposition => new[] {v0, v1};
        public override float[] vec => Mathf.normVec(new[] {A, S});

        public static BirdPersonality makeRandom() {
            var p = new BirdPersonality();
            // TODO: make this a more interesting curve
            var vfU = 0f;
            var vfS = 0.4f;
            p.S = Mathf.clamp(Distribution.normalRand(vfU, vfS), -1f, 1f);
            p.A = Mathf.clamp(Distribution.normalRand(vfU, vfS), -1f, 1f);
            // generate humor vector
            // TODO: this should be weighted based on other traits 
            // so that similar personalities tend toward similar humor
            var diVfU = 0f;
            var diVfS = 0.6f;
            p.v0 = Mathf.clamp(Distribution.normalRand(diVfU, diVfS), -1f, 1f);
            p.v1 = Mathf.clamp(Distribution.normalRand(diVfU, diVfS), -1f, 1f);

            return p;
        }

        public static BirdPersonality makeNeutral() {
            var p = new BirdPersonality();
            p.S = 0;
            p.A = 0;
            p.v0 = 0;
            p.v1 = 0;
            return p;
        }

        public override string ToString() {
            return $"[Personality A:{A}, S:{S}, v0:{v0}, v1:{v1}]";
        }
    }
}