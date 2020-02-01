using System;
using LunchtimeGears.AI.Utility;

namespace Sor.AI.Consid {
    public class HungerConsiderations {
        public class HungerAppraisal : Appraisal<Mind> {
            public HungerAppraisal(Mind context) : base(context) { }

            public override float score() {
                throw new NotImplementedException();
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