using Ducia.Calc;
using Ducia.Framework.Utility;
using Sor.AI.Cogs;

namespace Sor.AI.Consid {
    public static class ExploreAppraisals {
        public class ExplorationTendency : Appraisal<DuckMind> {
            public ExplorationTendency(DuckMind context) : base(context) { }

            public override float score() {
                return PerMath.map11to01(context.soul.traits.wary);
            }
        }

        public class Unexplored : Appraisal<DuckMind> {
            public Unexplored(DuckMind context) : base(context) { }
            public override float score() {
                return 0.6f;
            }
        }
    }
}