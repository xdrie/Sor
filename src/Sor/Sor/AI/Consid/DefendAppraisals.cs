using System.Linq;
using LunchtimeGears.AI.Utility;

namespace Sor.AI.Consid {
    public static class DefendAppraisals {
        public class NearbyThreatAppraisal : Appraisal<Mind> {
            public NearbyThreatAppraisal(Mind context) : base(context) { }

            public override float score() {
                return context.state.seenWings.Any(
                    x => context.state.getOpinion(x.mind) < MindConstants.OPINION_NEUTRAL)
                    ? 1
                    : 0;
            }
        }

        public class ThreatFightableAppraisal : Appraisal<Mind> {
            public ThreatFightableAppraisal(Mind context) : base(context) { }

            public override float score() {
                return 0.7f; // all threats are fightable now
            }
        }
    }
}