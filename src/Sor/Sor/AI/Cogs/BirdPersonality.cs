using LunchLib.Calc;
using LunchLib.Cogs;
using XNez.GUtils.Misc;

namespace Sor.AI.Cogs {
    public class BirdPersonality : Personality {
        public float A; // anxiety
        public float S; // sociability
        public float E; // emotionality
        
        public override float[] vec => new[] {A, S, E};

        // full names
        public float anxiety => A;
        public float social => S;
        public float emotionality => E;

        public override void generateRandom() {
            // generate personalities along a normal distribution
            S = normalRand(0.1f, 0.4f);
            A = normalRand(0.1f, 0.6f);
            E = normalRand(-0.2f, 0.4f);
        }

        public override string ToString() {
            return $"[A:{A:n2},S:{S:n2},E:{E:n2}]";
        }
    }
}