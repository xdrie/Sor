using System;
using System.Linq;
using Glint.AI.Misc;
using LunchtimeGears.AI.Utility;
using Sor.Components.Things;

namespace Sor.AI.Consid {
    public static class HungerAppraisals {
        public class HungerAppraisal : Appraisal<Mind> {
            public HungerAppraisal(Mind context) : base(context) { }

            public override float score() {
                // hunger score is based on the necessity of more energy.
                // let E be energy percentage (energy / max energy), clamp01
                // y = (1 - E)^2
                var energyCore = context.Entity.GetComponent<EnergyCore>();
                var energyPercentage = Gmathf.clamp01(energyCore.ratio);
                return energyPercentage * energyPercentage;
            }
        }

        public class FoodAvailabilityAppraisal : Appraisal<Mind> {
            public FoodAvailabilityAppraisal(Mind context) : base(context) { }

            public override float score() {
                // availability is based on nearby, known food items
                // TODO: look around the map for trees, ranked by level
                // for now, it is based on the existence of fruits nearby
                return context.state.seenThings.Any(x => x is Capsule) ? 1 : 0;
            }
        }
    }
}