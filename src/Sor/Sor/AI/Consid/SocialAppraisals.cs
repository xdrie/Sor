using System.Linq;
using Ducia.Calc;
using Ducia.Framework.Utility;
using MoreLinq;
using Nez;
using Sor.AI.Cogs;
using Sor.Components.Units;
using XNez.GUtils.Misc;

namespace Sor.AI.Consid {
    public static class SocialAppraisals {
        public class NearbyPotentialAllies : Appraisal<DuckMind> {
            public NearbyPotentialAllies(DuckMind context) : base(context) { }

            public static int opinionThreshold(DuckMind mind) {
                var prospective = mind.soul.traits.sociability > 0.6f;
                var thresh =
                    prospective ? Constants.DuckMind.OPINION_NEUTRAL - 50 : Constants.DuckMind.OPINION_NEUTRAL;
                return thresh;
            }

            public static Wing bestCandidate(DuckMind mind, int thresh) {
                // TODO: de-prioritize ducks we're already chums with
                var wings = mind.state.seenWings
                    .Where(x => mind.state.getOpinion(x.mind.state.me) > thresh) // above thresh
                    .MaxBy(x => mind.state.getOpinion(x.mind.state.me)); // highest opinion
                if (!wings.Any()) return null;

                return wings.First();
            }

            public override float score() {
                var thresh = opinionThreshold(context);
                var candidate = bestCandidate(context, thresh);
                if (candidate == null) return 0;
                // scale from 0-100
                return GMathf.map01clamp01(context.state.getOpinion(candidate.mind.state.me),
                    thresh, Constants.DuckMind.OPINION_ALLY);
            }
        }

        public class Sociability : Appraisal<DuckMind> {
            public Sociability(DuckMind context) : base(context) { }

            public override float score() {
                // scale [0,1] sociability on sqrt curve
                return Mathf.Sqrt(PerMath.map01(context.soul.traits.sociability));
            }
        }

        /// <summary>
        /// Energy availability for socializing
        /// </summary>
        public class FriendBudget : Appraisal<DuckMind> {
            public FriendBudget(DuckMind context) : base(context) { }

            public static float budget(DuckMind mind) {
                return mind.state.me.core.energy - mind.state.me.core.designMax * 0.8f;
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