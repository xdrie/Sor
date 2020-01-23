using Sor.Components.Things;

namespace Sor.AI.Signals {
    public class ItemSignals {
        public class CapsuleAcquiredSignal : MindSignal {
            public Capsule cap;

            public CapsuleAcquiredSignal(Capsule cap) {
                this.cap = cap;
            }
        }
    }
}