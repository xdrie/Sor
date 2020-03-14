using System.Linq;
using LunchLib.AI.Utility;
using MoreLinq;
using Sor.Components.Units;

namespace Sor.AI.Consid {
    public static class DefenseAppraisals {
        public class NearbyThreat : Appraisal<Mind> {
            public NearbyThreat(Mind context) : base(context) { }
            
            public static Wing greatestThreat(Mind mind) {
                // find the nearby duck with the lowest opinion
                // TODO: allow tracking multiple threats
                lock (mind.state.seenWings) {
                    var wings = mind.state.seenWings.MaxBy(
                        x => MindConstants.OPINION_NEUTRAL - mind.state.getOpinion(x.mind));
                    if (!wings.Any()) return null;

                    return wings.First();
                }
            }

            public override float score() {
                lock (context.state.seenWings) {
                    var threat = greatestThreat(context);
                    if (threat == null) return 0;
                    var threatOpinion = context.state.getOpinion(threat.mind);
                    // TODO: score the threat on a threshold for a 0-1 curve of threatening-ness
                    return 1;
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