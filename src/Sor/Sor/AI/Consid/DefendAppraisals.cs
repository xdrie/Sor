using System.Linq;
using LunchLib.AI.Utility;

namespace Sor.AI.Consid {
    public static class DefendAppraisals {
        public class NearbyThreat : Appraisal<Mind> {
            public NearbyThreat(Mind context) : base(context) { }

            public override float score() {
                lock (context.state.seenWings) {
                    return context.state.seenWings.Any(
                        x => context.state.getOpinion(x.mind) < MindConstants.OPINION_NEUTRAL)
                        ? 1
                        : 0;
                }
            }
        }

        public class ThreatFightable : Appraisal<Mind> {
            public ThreatFightable(Mind context) : base(context) { }

            public override float score() {
                // TODO: determine how fightable the threats are
                return 0.7f; // all threats are fightable now
            }
        }
    }
}