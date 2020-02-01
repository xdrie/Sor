using System;
using Glint.AI.Misc;
using LunchtimeGears.AI.Utility;
using Sor.Components.Things;

namespace Sor.AI.Consid {
    public class HungerConsiderations {
        public class HungerAppraisal : Appraisal<Mind> {
            public HungerAppraisal(Mind context) : base(context) { }
            
            public override float score() {
                // hunger score is based on the necessity of more energy.
                // let E be energy percentage (energy / max energy), clamp01
                // y = (1 - E)^2
                var energyCore = context.Entity.GetComponent<EnergyCore>();
                var energyPercentage = (float) (energyCore.energy / energyCore.designMax);
                energyPercentage = Gmathf.clamp01(energyPercentage);
                return energyPercentage * energyPercentage;
            }
        }

        public class FoodAvailabilityAppraisal : Appraisal<Mind> {
            public FoodAvailabilityAppraisal(Mind context) : base(context) { }

            public override float score() {
                throw new NotImplementedException();
            }
        }
    }
}