using Ducia.Layer1;
using Sor.Components.Things;

namespace Sor.AI.Signals {
    public class ItemSignals {
        public class CapsuleAcquiredSignal : MindSignal {
            public Capsule cap;
            public double energy;

            public CapsuleAcquiredSignal(Capsule cap, double energy) {
                this.cap = cap;
                this.energy = energy;
            }
        }
    }
}