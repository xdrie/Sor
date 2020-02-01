using Nez;

namespace Sor.Components.Things {
    /// <summary>
    /// Represents energy storage
    /// </summary>
    public class EnergyCore : Component {
        public float energy;
        public float designMax;

        public EnergyCore(float capacity) {
            energy = capacity;
            designMax = capacity;
        }
        
        public float ratio => energy / designMax;
    }
}