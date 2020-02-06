using LunchLib.AI.Utility;
using Sor.AI.Cogs;

namespace Sor.AI.Consid {
    public static class ExploreAppraisals {
        public class ExplorationTendency : Appraisal<Mind> {
            public ExplorationTendency(Mind context) : base(context) { }

            public override float score() {
                return PerMath.map11to01(context.soul.traits.wary);
            }
        }

        public class Unexplored : Appraisal<Mind> {
            public Unexplored(Mind context) : base(context) { }
            public override float score() {
                return 0.6f;
            }
        }
    }
}