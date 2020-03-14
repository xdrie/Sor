using Nez;

namespace Sor.Components.Things {
    /// <summary>
    /// Represents energy storage
    /// </summary>
    public class EnergyCore : Component {
        public float energy;
        public float designMax;
        public float overloadThreshold = 1.4f;

        public EnergyCore(float capacity) {
            energy = capacity;
            designMax = capacity;
        }
        
        public float ratio => energy / designMax;

        public float overloadedNess() => 1 - Mathf.Pow((ratio - overloadThreshold + 1), -2);

        public void fill() => energy = designMax;

        public void clamp() {
            if (energy < 0f) {
                energy = 0f;
            }
        }
    }
}