using Ducia.Cogs;

namespace Sor.AI.Cogs {
    public class BirdPersonality : Personality {
        public float A; // anxiety
        public float S; // sociability
        public float E; // emotionality

        public override float[] vec => new[] {A, S, E};

        public BirdPersonality() { }

        public BirdPersonality(float a, float s, float e) {
            A = a;
            S = s;
            E = e;
        }

        public override string ToString() {
            return $"[A:{A:n2},S:{S:n2},E:{E:n2}]";
        }

        public static BirdPersonality makeRandom() {
            // generate personalities along a normal distribution
            return new BirdPersonality(a: normalRand(0.1f, 0.6f), s: normalRand(0.1f, 0.4f),
                e: normalRand(-0.2f, 0.4f));
        }

        public static BirdPersonality makeNeutral() {
            // generate personalities along a normal distribution
            return new BirdPersonality(a: 0, s: 0, e: 0);
        }
    }
}