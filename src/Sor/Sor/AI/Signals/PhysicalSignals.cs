using Ducia.Layer1;
using Microsoft.Xna.Framework;
using Sor.Components.Items;
using Sor.Components.Units;

namespace Sor.AI.Signals {
    public class PhysicalSignals {
        public class ShotSignal : MindSignal {
            public Shooter gun;

            public ShotSignal(Shooter gun) {
                this.gun = gun;
            }
        }

        public class BumpSignal : MindSignal {
            public Wing wing;
            public Vector2 impact;

            public BumpSignal(Wing wing, Vector2 impact) {
                this.wing = wing;
                this.impact = impact;
            }
        }
    }
}