using LunchLib.Calc;
using LunchLib.Cogs;
using XNez.GUtils.Misc;

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
        public override float[] vec => GMathf.normVec(new[] {A, S});

        public override void generateRandom() {
            // TODO: make this a more interesting curve
            var vfU = 0f;
            var vfS = 0.4f;
            S = GMathf.clamp(Distribution.normalRand(vfU, vfS), -1f, 1f);
            A = GMathf.clamp(Distribution.normalRand(vfU, vfS), -1f, 1f);
            // generate humor vector
            // TODO: this should be weighted based on other traits 
            // so that similar personalities tend toward similar humor
            var diVfU = 0f;
            var diVfS = 0.6f;
            v0 = GMathf.clamp(Distribution.normalRand(diVfU, diVfS), -1f, 1f);
            v1 = GMathf.clamp(Distribution.normalRand(diVfU, diVfS), -1f, 1f);
        }
        
        public override string ToString() {
            return $"[A:{A:n2},S:{S:n2},v0:{v0:n2},v1:{v1:n2}]";
        }
    }
}