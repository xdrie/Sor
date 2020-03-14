using System.Linq;
using LunchLib.AI.Utility;
using MoreLinq;
using Nez;
using Sor.AI.Cogs;
using Sor.Components.Units;
using XNez.GUtils.Misc;

namespace Sor.AI.Consid {
    public static class SocialAppraisals {
        public class NearbyPotentialAllies : Appraisal<Mind> {
            public NearbyPotentialAllies(Mind context) : base(context) { }

            public static int opinionThreshold(Mind mind) {
                var prospective = mind.soul.traits.sociability > 0.6f;
                var thresh =
                    prospective ? MindConstants.OPINION_NEUTRAL - 50 : MindConstants.OPINION_NEUTRAL;
                return thresh;
            }

            public static Wing bestCandidate(Mind mind, int thresh) {
                // TODO: de-prioritize ducks we're already chums with
                lock (mind.state.seenWings) {
                    var wings = mind.state.seenWings.MaxBy(
                        x => mind.state.getOpinion(x.mind) - thresh);
                    if (!wings.Any()) return null;

                    return wings.First();
                }
            }

            public override float score() {
                var thresh = opinionThreshold(context);
                var candidate = bestCandidate(context, thresh);
                if (candidate == null) return 0;
                // scale from 0-100
                return GMathf.map01clamp01(context.state.getOpinion(candidate.mind),
                    thresh, MindConstants.OPINION_ALLY);
            }
        }

        public class Sociability : Appraisal<Mind> {
            public Sociability(Mind context) : base(context) { }

            public override float score() {
                // scale [0,1] sociability on sqrt curve
                return Mathf.Sqrt(PerMath.map11to01(context.soul.traits.sociability));
            }
        }

        /// <summary>
        /// Energy availability for socializing
        /// </summary>
        public class FriendBudget : Appraisal<Mind> {
            public FriendBudget(Mind context) : base(context) { }

            public static float budget(Mind mind) {
                return mind.me.core.energy - mind.me.core.designMax * 0.8f;
            }

            public override float score() {
                // allocate energy for budget
                if (budget(context) > 0) {
                    // TODO: make this scale along a curve
                    return 1;
                }

                return 0;
            }
        }
    }
}