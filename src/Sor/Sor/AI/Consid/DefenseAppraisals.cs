using System.Linq;
using LunchLib.AI.Utility;
using LunchLib.Calc;
using MoreLinq;
using Nez;
using Sor.Components.Units;

namespace Sor.AI.Consid {
    public static class DefenseAppraisals {
        public class NearbyThreat : Appraisal<Mind> {
            public NearbyThreat(Mind context) : base(context) { }

            public static int threatThreshold(Mind mind) {
                // threat threshold (min opinion to be threat) depends on personality
                // threshold range: [-100, 70]
                return (int) GMathExt.transformTrait(mind.soul.traits.aggression,
                    -120, 70, -100, 70);
            }

            public static Wing greatestThreat(Mind mind) {
                // find the nearby duck with the lowest opinion
                // TODO: allow tracking multiple threats
                lock (mind.state.seenWings) {
                    var wings = mind.state.seenWings.MaxBy(
                        x => threatThreshold(mind) - mind.state.getOpinion(x.mind));
                    if (!wings.Any()) return null;

                    return wings.First();
                }
            }

            private float threateningNess(int opinionDelta) {
                // how threatened this opinion delta makes us feel
                var t_threat = context.soul.traits.aggression; // threat sensitivity [0,1]
                return
                    1 -
                    Mathf.Pow(
                        opinionDelta / ((1 - t_threat) * 100) - 1
                        , -2);
            }

            public override float score() {
                lock (context.state.seenWings) {
                    var threatWing = greatestThreat(context);
                    if (threatWing == null) return 0;
                    var threatOpinion = context.state.getOpinion(threatWing.mind);
                    var threatValue = threateningNess(threatThreshold(context) - threatOpinion);
                    return threatValue;
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