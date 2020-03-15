using System;
using System.Linq;
using LunchLib.AI.Utility;
using LunchLib.Calc;
using Microsoft.Xna.Framework;
using MoreLinq;
using Nez;
using Sor.Components.Items;
using Sor.Components.Units;
using XNez.GUtils.Misc;

namespace Sor.AI.Consid {
    public static class DefenseAppraisals {
        public class NearbyThreat : Appraisal<Mind> {
            public NearbyThreat(Mind context) : base(context) { }

            public static int threatThreshold(Mind mind) {
                // threat threshold (min opinion to be threat) depends on personality
                // threshold range: [-100, 70]
                return (int) TraitCalc.transformTrait(mind.soul.traits.aggression,
                    -120, 70, -100, 70);
            }

            public static Wing greatestThreat(Mind mind) {
                // find the nearby duck with the lowest opinion
                // TODO: allow tracking multiple threats
                lock (mind.state.seenWings) {
                    var wings = mind.state.seenWings
                        .Where(x => mind.state.getOpinion(x.mind) < threatThreshold(mind)) // below thresh
                        .MinBy(x => mind.state.getOpinion(x.mind)); // lowest opinion
                    if (!wings.Any()) return null;

                    return wings.First();
                }
            }

            private float threateningNess(int opinionDelta) {
                // how threatened this opinion delta makes us feel (operates on negative opinion deltas)
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
                    var threatValue = threateningNess(threatOpinion - threatThreshold(context));
                    return threatValue;
                }
            }
        }

        public class ThreatFightable : Appraisal<Mind> {
            public ThreatFightable(Mind context) : base(context) { }

            private int scoreRatio(float ratio, int weight) {
                var score = LCurves.ratioAdvantage(ratio, 1.2f);
                return (int) (score * weight);
            }

            public override float score() {
                // determine how fightable nearby threats are
                var threat = NearbyThreat.greatestThreat(context);
                if (threat == null) return 0; // no threat means don't score

                // TODO: use better functions to map ratios to score
                // 1. compare core sizes (ratio) -> transform [-40, 40]
                var coreSizeRatio = context.me.core.designMax / threat.mind.me.core.designMax;
                var coreSizeScore = scoreRatio(1 / coreSizeRatio, 20);

                // 2. compare maneuverability (ratio) -> transform [-20, 20]
                // if we're more maneuverable, it might not be worth fighting
                var maneuverabilityRatio = context.me.body.turnPower / threat.mind.me.body.turnPower;
                var maneuScore = scoreRatio(maneuverabilityRatio, 20);

                // 3. compare speed (ratio) -> transform [-40, 40]
                // if we're faster, we want to fight less
                var speedRatio = context.me.body.thrustPower / threat.mind.me.body.thrustPower;
                var speedScore = scoreRatio(speedRatio, 30);

                // 3. compare energy (ratio) -> transform [-40, 40]
                // if we're faster, we want to fight less
                var energyRatio = context.me.core.energy / threat.mind.me.core.energy;
                var energyScore = scoreRatio(1 / energyRatio, 30);
                
                // 4. compare armed state
                var threatWeaponMaxscore = 40;
                var threatWeapon = threat.mind.GetComponent<Shooter>();
                var myWeapon = context.me.GetComponent<Shooter>();
                // if they're armed, set negative score
                var armoryScore = threatWeapon == null ? 0 : -threatWeaponMaxscore;
                // TODO: compare weapons
                if (myWeapon != null) { // if i'm armed too
                    // then negate the score penalty
                    armoryScore += threatWeaponMaxscore;
                }

                // clamp score to [-100, 100] -> transform [0, 1]
                var score = coreSizeScore + maneuScore + speedScore + energyScore + armoryScore;

                context.state.setBoard("judged threat",
                    new MindState.BoardItem($"E:{energyScore}, C:{coreSizeScore}, M:{maneuScore}, S:{speedScore}",
                        "interaction",
                        Color.Orange, Time.TotalTime + 1f));

                var clampedScore = GMathf.clamp(score, -100, 100); // clamp score
                var fightable = GMathf.map01(clampedScore, -100, 100); // map to [-1,1]
                return fightable;
            }
        }
    }
}