using LunchtimeGears.AI.Utility;

namespace Sor.AI.Consid {
    public static class ExploreAppraisals {
        public class ExplorationTendencyAppraisal : Appraisal<Mind> {
            public ExplorationTendencyAppraisal(Mind context) : base(context) { }

            public override float score() {
                return 0.8f;
            }
        }

        public class UnexploredAppraisal : Appraisal<Mind> {
            public UnexploredAppraisal(Mind context) : base(context) { }
            public override float score() {
                return 0.8f;
            }
        }
    }
}