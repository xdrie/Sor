using System.Linq;
using LunchLib.AI.Utility;
using Sor.Components.Things;
using XNez.GUtils.Misc;

namespace Sor.AI.Consid {
    public static class HungerAppraisals {
        public class Hunger : Appraisal<Mind> {
            public Hunger(Mind context) : base(context) { }

            public override float score() {
                // hunger score is based on the necessity of more energy.
                // let E be energy percentage (energy / max energy), clamp01
                // y = (1 - E)^2
                var energyCore = context.Entity.GetComponent<EnergyCore>();
                var invEnergyPerc = 1 - GMathf.clamp01(energyCore.ratio);
                return GMathf.sqrt(invEnergyPerc * invEnergyPerc);
            }
        }

        public class FoodAvailability : Appraisal<Mind> {
            public FoodAvailability(Mind context) : base(context) { }

            public override float score() {
                // availability is based on nearby, known food items
                // TODO: look around the map for trees, ranked by level
                // for now, it is based on the existence of fruits nearby
                lock (context.state.seenThings) {
                    return context.state.seenThings.Any(x => x is Capsule) ? 1 : 0;
                }
            }
        }
    }
}