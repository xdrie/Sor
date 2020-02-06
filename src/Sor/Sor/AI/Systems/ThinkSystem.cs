using System.Linq;
using System.Threading;
using Activ.GOAP;
using LunchLib.AI.Utility;
using LunchLib.AI.Utility.Considerations;
using Sor.AI.Cogs.Interactions;
using Sor.AI.Consid;
using Sor.AI.Plan;
using Sor.AI.Signals;
using Sor.Components.Things;

namespace Sor.AI.Systems {
    public class ThinkSystem : MindSystem {
        public int maxSignalsPerThink = 40;

        public ThinkSystem(Mind mind, float refresh, CancellationToken cancelToken) :
            base(mind, refresh, cancelToken) { }

        protected override void process() {
            // look at signals
            var processedSignals = 0;
            while (state.signalQueue.TryDequeue(out var signal)) {
                // process the signal
                processSignal(signal);

                processedSignals++;
                if (processedSignals > maxSignalsPerThink) break;
            }

            // update soul systems
            mind.soul.tick();

            // look at mind information
            thinkVisual();

            // figure out plans
            makePlans();
        }

        private void makePlans() {
            // create utility planner
            var reasoner = new Reasoner<Mind>();

            var eatConsideration = new ThresholdConsideration<Mind>(() => { // eat action
                var hungryPlanModel = new HungryBird();
                var hungrySolver = new Solver<HungryBird>();
                var seenBeans = state.seenThings.Where(x => x is Capsule).ToList();
                // update the model
                hungryPlanModel.nearbyBeans = seenBeans.Count;

                // TODO: tweak this so it syncs up with the reasoner selecting the objective
                var targetSatiety = state.mind.me.body.metabolicRate * 15f; // 15 seconds of food
                var next = hungrySolver.Next(hungryPlanModel, new Goal<HungryBird>(x => x.satiety > targetSatiety, null));
                // TODO: interpret action plan
                lock (state.targetQueue) {
                    state.targetQueue.Clear();
                    var path = next.Path();
                    foreach (var node in path) {
                        // handle planning based on the node
                        if ((string) node.action == nameof(HungryBird.eatBean)) { // plan eating the nearest bean
                            // TODO: add the bean to the target entity queue
                            var bean = seenBeans[0];
                            seenBeans.Remove(bean);
                            state.targetQueue.Enqueue(bean.Entity);
                        } else if ((string) node.action == nameof(HungryBird.visitTree)) {
                            // plan to visit the nearest tree
                            // TODO: how is this done?
                        }
                    }
                }
            }, 0.6f, "eat");
            eatConsideration.addAppraisal(new HungerAppraisals.Hunger(mind)); // 0-1
            eatConsideration.addAppraisal(new HungerAppraisals.FoodAvailability(mind)); //0-1
            eatConsideration.scale = 1 / 2f;
            reasoner.addConsideration(eatConsideration);

            var exploreConsideration = new SumConsideration<Mind>(() => {
                // explore action
                // TODO: actually use map knowledge to explore
            }, "explore");
            exploreConsideration.addAppraisal(new ExploreAppraisals.ExplorationTendency(mind));
            exploreConsideration.addAppraisal(new ExploreAppraisals.Unexplored(mind));
            exploreConsideration.scale = 1 / 2f;
            reasoner.addConsideration(exploreConsideration);

            var defendConsideration = new ThresholdSumConsideration<Mind>(() => {
                // defend action
                // TODO: figure out the most "threatening" wing, delegate to goal planner
                var tgtWing = state.seenWings.FirstOrDefault(
                    x => state.getOpinion(x.mind) < MindConstants.OPINION_NEUTRAL);
                if (tgtWing != null) {
                    state.target = tgtWing.body.pos;
                }
            }, 0.8f, "defend");
            defendConsideration.addAppraisal(new DefendAppraisals.NearbyThreat(mind));
            defendConsideration.addAppraisal(new DefendAppraisals.ThreatFightable(mind));
            defendConsideration.scale = 1 / 2f;
            reasoner.addConsideration(defendConsideration);

            var socialAppraisal = new SumConsideration<Mind>(() => {
                // socialize
                // TODO: attempt to feed a duck
            }, "social");
            socialAppraisal.addAppraisal(new SocialAppraisals.NearbyPotentialAllies(mind));
            socialAppraisal.addAppraisal(new SocialAppraisals.Sociability(mind));
            socialAppraisal.scale = 1 / 2f;
            reasoner.addConsideration(socialAppraisal);

            var resultTable = reasoner.execute();
            if (mind.state.lastPlanTable == null) {
                state.lastPlanTable = resultTable;
            } else {
                lock (mind.state.lastPlanTable) {
                    state.lastPlanTable = resultTable;
                }
            }

            var chosen = reasoner.choose(resultTable);
            // run action
            chosen.action();
        }

        private void processSignal(MindSignal result) {
            switch (result) {
                case ItemSignals.CapsuleAcquiredSignal sig: {
                    var from = sig.cap.sender;
                    if (from != null && from != mind.me) {
                        // run a feeding interaction
                        var interaction = new CapsuleFeedingInteraction(sig);
                        interaction.run(mind.soul, from.mind.soul);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Think about visual data available to me.
        /// </summary>
        private void thinkVisual() {
            // look at wings and their distances to me.
            lock (state.seenWings) {
                foreach (var wing in state.seenWings) {
                    var toWing = entity.Position - wing.Entity.Position;
                    var toWingDist = toWing.Length();
                    if (toWingDist <= NearbyBirdInteraction.nearRange) {
                        var interaction = new NearbyBirdInteraction(toWingDist);
                        interaction.run(mind.soul, wing.mind.soul);
                    }
                }
            }
        }
    }
}