using System.Linq;
using LunchLib.AI.Utility;
using Sor.AI.Cogs;
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

            public override float score() {
                // TODO: a more prospective way to look for friendships
                var thresh = opinionThreshold(context);
                var wings = context.state.seenWings.Where(
                        x => context.state.getOpinion(x.mind) > thresh)
                    .OrderByDescending(x => context.state.getOpinion(x.mind)).ToList();
                if (!wings.Any()) return 0;
                // scale from 0-100
                var firstWing = wings.First();
                return GMathf.map01clamp01(context.state.getOpinion(firstWing.mind),
                    thresh, MindConstants.OPINION_ALLY);
            }
        }

        public class Sociability : Appraisal<Mind> {
            public Sociability(Mind context) : base(context) { }

            public override float score() {
                return PerMath.map11to01(context.soul.traits.sociability);
            }
        }

        /// <summary>
        /// Energy availability for socializing
        /// </summary>
        public class FriendBudget : Appraisal<Mind> {
            public FriendBudget(Mind context) : base(context) { }

            public override float score() {
                // allocate 2000 energy
                var budget = 2000f;
                if (context.me.core.energy - budget > 0.7f * context.me.core.designMax) {
                    return 1;
                }

                return 0;
            }
        }
    }
}