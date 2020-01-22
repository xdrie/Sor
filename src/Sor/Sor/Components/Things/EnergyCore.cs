using Nez;

namespace Sor.Components.Things {
    /// <summary>
    /// Represents energy storage
    /// </summary>
    public class EnergyCore : Component {
        public double energy;
        public double designMax;

        public EnergyCore(double capacity) {
            energy = capacity;
            designMax = capacity;
        }
        
        public double ratio => energy / designMax;
    }
}