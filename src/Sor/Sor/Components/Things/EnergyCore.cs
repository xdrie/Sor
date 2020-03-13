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

        public float overloadedNess() => Mathf.Pow((1 - (ratio - overloadThreshold + 1)), -2);
    }
}