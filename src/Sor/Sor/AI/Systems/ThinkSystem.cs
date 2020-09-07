using System.Threading;
using Ducia;
using Ducia.Framework.Utility;
using Ducia.Framework.Utility.Considerations;
using Ducia.Systems;
using Nez;
using Sor.AI.Cogs.Interactions;
using Sor.AI.Consid;
using Sor.AI.Signals;
using Sor.Components.Units;

namespace Sor.AI.Systems {
    public partial class ThinkSystem : PlannerSystem<DuckMind, DuckMindState> {
        private Reasoner<DuckMind> reasoner;

        public ThinkSystem(DuckMind mind, float refresh, CancellationToken cancelToken) :
            base(mind, refresh, cancelToken) {
            createReasoner();
        }

        private void createReasoner() {
            // create utility planner
            reasoner = new Reasoner<DuckMind>();
            reasoner.scoreType = Reasoner<DuckMind>.ScoreType.Normalized;

            var eatConsideration = new ThresholdConsideration<DuckMind>(eatAction, 0.3f, "eat");
            eatConsideration.addAppraisal(new EatAppraisals.Hunger(mind)); // 0-1
            eatConsideration.addAppraisal(new EatAppraisals.FoodAvailability(mind)); //0-1
            reasoner.addConsideration(eatConsideration);

            var exploreConsideration = new SumConsideration<DuckMind>(exploreAction, "explore");
            exploreConsideration.addAppraisal(new ExploreAppraisals.ExplorationTendency(mind));
            exploreConsideration.addAppraisal(new ExploreAppraisals.Unexplored(mind));
            reasoner.addConsideration(exploreConsideration);

            // FIGHT of fight-or-flight
            var fightConsideration = new ThresholdSumConsideration<DuckMind>(fightAction, 0.8f, "fight");
            fightConsideration.addAppraisal(new DefenseAppraisals.NearbyThreat(mind));
            fightConsideration.addAppraisal(new DefenseAppraisals.ThreatFightable(mind));
            reasoner.addConsideration(fightConsideration);

            // FLIGHT of fight-or-flight
            var fleeConsideration = new ThresholdConsideration<DuckMind>(fleeAction, 0.4f, "flee");
            fleeConsideration.addAppraisal(new DefenseAppraisals.NearbyThreat(mind));
            fleeConsideration.addAppraisal(new DefenseAppraisals.ThreatFightable(mind).inverse());
            reasoner.addConsideration(fleeConsideration);

            var socialConsideration = new ThresholdConsideration<DuckMind>(socialAction, 0.2f, "social");
            socialConsideration.addAppraisal(new SocialAppraisals.NearbyPotentialAllies(mind));
            socialConsideration.addAppraisal(new SocialAppraisals.Sociability(mind));
            socialConsideration.addAppraisal(new SocialAppraisals.FriendBudget(mind));
            reasoner.addConsideration(socialConsideration);
        }

        protected override void makePlans() {
            // run the utility ai planner
            var resultTable = reasoner.execute();
            // store plan log
            state.updatePlanLog(resultTable);

            var chosen = reasoner.choose(resultTable); // pick the best-scored option
            chosen.action(); // execute the action
        }

        protected override bool processSignal(MindSignal result) {
            switch (result) {
                case ItemSignals.CapsuleAcquiredSignal sig: {
                    var from = sig.cap.interactor;
                    if (from != null && from != mind.state.me) {
                        // run a feeding interaction
                        var interaction = new FeedInteraction(sig);
                        interaction.run(mind, from.mind);
                    }

                    return true;
                }
                case PhysicalSignals.ShotSignal sig: {
                    var from = sig.gun.Entity.GetComponent<Wing>();
                    if (from != null && from != mind.state.me) {
                        // run a being shot interaction
                        var interaction = new ShotInteraction(from, sig);
                        interaction.run(mind, from.mind);
                    }

                    return true;
                }
                case PhysicalSignals.BumpSignal sig: {
                    var from = sig.wing;
                    if (from != null && from != mind.state.me) {
                        // run an interaction for being bumped
                        var interaction = new BumpInteraction(sig);
                        interaction.run(mind, from.mind);
                    }

                    return true;
                }

                default:
                    return false;
            }
        }

        protected override void processSenses() {
            // look at wings and their distances to me
            foreach (var wing in state.seenWings) {
                var toWing = mind.entity.Position - wing.Entity.Position;
                // subtract diag hitbox
                var hitboxRadSq = wing.hitbox.Width * wing.hitbox.Width + wing.hitbox.Height * wing.hitbox.Height;
                var toWingDist = Mathf.Sqrt(toWing.LengthSquared() + hitboxRadSq);
                if (toWingDist <= NearbyInteraction.triggerRange) {
                    var interaction = new NearbyInteraction(toWingDist);
                    interaction.run(mind, wing.mind);
                }
            }
        }
    }
}