using System.Linq;
using LunchLib.AI.Utility;
using MoreLinq;
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
                var wings = mind.state.seenWings.MaxBy(
                    x => mind.state.getOpinion(x.mind) > thresh);
                if (!wings.Any()) return null;
                return wings.First();
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
                return PerMath.map11to01(context.soul.traits.sociability);
            }
        }
    }
}