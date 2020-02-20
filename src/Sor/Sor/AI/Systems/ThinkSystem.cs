using System.Linq;
using System.Threading;
using Activ.GOAP;
using LunchLib.AI.Utility;
using LunchLib.AI.Utility.Considerations;
using MoreLinq.Extensions;
using Nez;
using Nez.AI.Pathfinding;
using Sor.AI.Cogs.Interactions;
using Sor.AI.Consid;
using Sor.AI.Model;
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
            reasoner.scoreType = Reasoner<Mind>.ScoreType.Normalized;

            var eatConsideration = new ThresholdConsideration<Mind>(() => { // eat action
                var hungryPlanModel = new HungryBird();
                var hungrySolver = new Solver<HungryBird>();
                var seenBeans = state.seenThings.Where(x => x is Capsule).ToList();
                // update the model
                hungryPlanModel.nearbyBeans = seenBeans.Count;

                // TODO: tweak this so it syncs up with the reasoner selecting the objective
                var targetSatiety = state.mind.me.body.metabolicRate * 15f; // 15 seconds of food
                var next = hungrySolver.Next(hungryPlanModel,
                    new Goal<HungryBird>(x => x.satiety > targetSatiety, null));
                // TODO: interpret action plan
                lock (state.plan) {
                    state.plan.Clear();
                    var path = next.Path();
                    foreach (var node in path) {
                        // handle planning based on the node
                        var timePerBean = 5f;
                        var beanTimeAcc = Time.TotalTime;
                        if ((string) node.action == nameof(HungryBird.eatBean)) { // plan eating the nearest bean
                            // TODO: add the bean to the target entity queue
                            var bean = seenBeans[0];
                            seenBeans.Remove(bean);
                            beanTimeAcc += timePerBean;
                            state.plan.Enqueue(new EntityTargetSource(bean.Entity, Approach.Precise, beanTimeAcc));
                        } else if ((string) node.action == nameof(HungryBird.visitTree)) {
                            // plan to visit the nearest tree
                            // TODO: how is this done?
                        }
                    }
                }
            }, 0.6f, "eat");
            eatConsideration.addAppraisal(new HungerAppraisals.Hunger(mind)); // 0-1
            eatConsideration.addAppraisal(new HungerAppraisals.FoodAvailability(mind)); //0-1
            reasoner.addConsideration(eatConsideration);

            var exploreConsideration = new SumConsideration<Mind>(() => {
                // explore action
                // TODO: a more interesting/useful explore action
                // attempt to do a room-to-room pathfind
                // get the nearest room
                var nearestRoom =
                    mind.gameCtx.map.roomGraph.rooms.MinBy(x =>
                            (mind.me.body.pos - x.center.ToVector2()).LengthSquared())
                        .First();
                // choose any room other than the nearest
                var goalRoom = mind.gameCtx.map.roomGraph.rooms
                    .Where(x => x != nearestRoom).RandomSubset(1)
                    .First();
                var foundPath = WeightedPathfinder.Search(mind.gameCtx.map.roomGraph, nearestRoom, goalRoom);
                // TODO: actually use map knowledge to explore
            }, "explore");
            exploreConsideration.addAppraisal(new ExploreAppraisals.ExplorationTendency(mind));
            exploreConsideration.addAppraisal(new ExploreAppraisals.Unexplored(mind));
            reasoner.addConsideration(exploreConsideration);

            var defendConsideration = new ThresholdSumConsideration<Mind>(() => {
                // defend action
                // TODO: figure out the most "threatening" wing, delegate to goal planner
                var tgtWing = state.seenWings.FirstOrDefault(
                    x => state.getOpinion(x.mind) < MindConstants.OPINION_NEUTRAL);
                if (tgtWing != null) {
                    lock (state.plan) {
                        state.plan.Clear(); // reset targets
                        state.plan.Enqueue(new EntityTargetSource(tgtWing.Entity));
                    }
                }
            }, 0.8f, "defend");
            defendConsideration.addAppraisal(new DefendAppraisals.NearbyThreat(mind));
            defendConsideration.addAppraisal(new DefendAppraisals.ThreatFightable(mind));
            reasoner.addConsideration(defendConsideration);

            var socialConsideration = new ThresholdConsideration<Mind>(() => {
                // socialize - attempt to feed a duck
                // pick a potential fren
                // TODO: don't choose ducks we're already chums with
                var candidates = mind.state.seenWings.Where(
                        x => mind.state.getOpinion(x.mind) > MindConstants.OPINION_NEUTRAL)
                    .OrderByDescending(x => mind.state.getOpinion(x.mind)).ToList();
                var fren = candidates.First();
                // add the fren as a close-range approach
                lock (state.plan) {
                    state.plan.Clear();
                    var feedTime = 10f;
                    var goalFeedTime = Time.TotalTime + feedTime;
                    state.plan.Enqueue(new EntityTargetSource(fren.Entity, Approach.Within, TargetSource.RANGE_SHORT, goalFeedTime));
                    // if we're close enough to our fren, feed them
                    var toFren = mind.me.body.pos - fren.body.pos;
                    // tell it to feed
                    state.plan.Enqueue(new PlanFeed(fren.Entity, goalFeedTime));
                }
            }, 0.2f, "social");
            socialConsideration.addAppraisal(new SocialAppraisals.NearbyPotentialAllies(mind));
            socialConsideration.addAppraisal(new SocialAppraisals.Sociability(mind));
            socialConsideration.addAppraisal(new SocialAppraisals.FriendBudget(mind));
            reasoner.addConsideration(socialConsideration);

            var resultTable = reasoner.execute();
            if (mind.state.lastPlanTable == null) {
                // ReSharper disable once InconsistentlySynchronizedField - it is null
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