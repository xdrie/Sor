using System.Linq;
using Glint.AI.Misc;
using LunchLib.AI.Utility;
using Sor.AI.Cogs;

namespace Sor.AI.Consid {
    public static class SocialAppraisals {
        public class NearbyPotentialAllies : Appraisal<Mind> {
            public NearbyPotentialAllies(Mind context) : base(context) { }

            public override float score() {
                // TODO: a more prospective way to look for friendships
                var wings = context.state.seenWings.Where(
                        x => context.state.getOpinion(x.mind) > MindConstants.OPINION_NEUTRAL)
                    .OrderByDescending(x => context.state.getOpinion(x.mind)).ToList();
                if (!wings.Any()) return 0;
                // scale from 0-100
                var firstWing = wings.First();
                return Gmathf.map01(context.state.getOpinion(firstWing.mind),
                    MindConstants.OPINION_NEUTRAL, MindConstants.OPINION_ALLY);
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