using System.Collections.Generic;
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
using Sor.AI.Plans;
using Sor.AI.Signals;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Game.Map;
using Sor.Util;

namespace Sor.AI.Systems {
    public class ThinkSystem : MindSystem {
        private int maxSignalsPerThink = 40;
        private Reasoner<Mind> reasoner;

        public ThinkSystem(Mind mind, float refresh, CancellationToken cancelToken) :
            base(mind, refresh, cancelToken) {
            createReasoner();
        }

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

        private void createReasoner() {
            // create utility planner
            reasoner = new Reasoner<Mind>();
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
                if (next == null) { // planning failed
                    return;
                }

                var newPlan = new List<PlanTask>();
                var path = next.Path();
                foreach (var node in path) {
                    // handle planning based on the node
                    var timePerBean = 5f;
                    var beanTimeAcc = Time.TotalTime;
                    if (node.matches(nameof(HungryBird.eatBean))) { // plan eating the nearest bean
                        // TODO: add the bean to the target entity queue
                        var bean = seenBeans[0];
                        seenBeans.Remove(bean);
                        beanTimeAcc += timePerBean;
                        newPlan.Add(new EntityTarget(mind, bean.Entity, Approach.Precise, beanTimeAcc));
                    } else if (node.matches(nameof(HungryBird.visitTree))) {
                        // plan to visit the nearest tree
                        // TODO: how is this done?
                    }
                }

                state.setPlan(newPlan);
            }, 0.6f, "eat");
            eatConsideration.addAppraisal(new HungerAppraisals.Hunger(mind)); // 0-1
            eatConsideration.addAppraisal(new HungerAppraisals.FoodAvailability(mind)); //0-1
            reasoner.addConsideration(eatConsideration);

            var exploreConsideration = new SumConsideration<Mind>(() => {
                // explore action
                // TODO: a more interesting/useful explore action
                // don't pathfind if we already have a valid path
                lock (state) {
                    if (state.navPath != null) {
                        if (state.isPlanValid) return;
                    }
                }

                // cancel if no map model
                if (mind.gameCtx.map == null) return;

                // attempt to do a room-to-room pathfind
                // attempt to pathfind using the structural navigation graph
                // get the nearest node
                var nearestNode = mind.gameCtx.map.sng.nodes.MinBy(x =>
                        (mind.me.body.pos - mind.gameCtx.map.tmxMap.TileToWorldPosition(x.pos.ToVector2()))
                        .LengthSquared())
                    .First();
                // get the nearest room
                var nearestRoom =
                    mind.gameCtx.map.roomGraph.rooms.MinBy(x =>
                            (mind.me.body.pos - mind.gameCtx.map.tmxMap.TileToWorldPosition(x.center.ToVector2()))
                            .LengthSquared())
                        .First();
                // TODO: navigate by room, not by node
                // choose a goal room by randomly walking the graph
                // var goalNode = mind.gameCtx.map.sng.nodes.Single(x => x.room == goalRoom);
                var goalNode = nearestNode;
                var visited = new Dictionary<StructuralNavigationGraph.Node, bool>();
                var walkDist = 6;
                for (var i = 0; i < walkDist; i++) {
                    visited[goalNode] = true;
                    var validLinks = goalNode.links?
                        .Where(x => !(visited.ContainsKey(x) && visited[x]));
                    if (validLinks != null && validLinks.Any()) {
                        goalNode = validLinks.RandomSubset(1).SingleOrDefault();
                    } else {
                        break; // we could not walk any further
                    }
                }

                var foundPath = AStarPathfinder.Search(mind.gameCtx.map.sng, nearestNode, goalNode);
                if (foundPath == null || !foundPath.Any()) return; // pathfind failed
                lock (state) {
                    state.navPath = foundPath;
                }

                // TODO: actually use map knowledge to explore
                // queue the points of the map

                var newPlan = new List<PlanTask>();
                foreach (var pathNode in foundPath) {
                    var tmapPos = pathNode.pos.ToVector2();
                    newPlan.Add(new FixedTarget(
                        mind, mind.gameCtx.map.tmxMap.TileToWorldPosition(tmapPos), Approach.Within,
                        TargetSource.RANGE_DIRECT));
                }

                state.setPlan(newPlan);

                var nextPt = foundPath.First().pos;
                state.setBoard("exp",
                    new MindState.BoardItem($"({nextPt.X}, {nextPt.Y} path[{foundPath.Count}])", "path"));
            }, "explore");
            exploreConsideration.addAppraisal(new ExploreAppraisals.ExplorationTendency(mind));
            exploreConsideration.addAppraisal(new ExploreAppraisals.Unexplored(mind));
            reasoner.addConsideration(exploreConsideration);

            // FIGHT of fight-or-flight
            var fightConsideration = new ThresholdSumConsideration<Mind>(() => {
                // fight threat nearby
                // TODO: figure out the most "threatening" wing, delegate to goal planner
                var threat = DefenseAppraisals.NearbyThreat.greatestThreat(mind);
                if (threat != null) {
                    // TODO: improve fighting/engagement, delegate to action planner
                    // follow and attack them
                    state.setPlan(new PlanTask[] {
                        new EntityTarget(mind, threat.Entity, Approach.Within, TargetSource.RANGE_SHORT),
                        new PlanAttack(mind, entity),
                    });
                }
            }, 0.8f, "fight");
            fightConsideration.addAppraisal(new DefenseAppraisals.NearbyThreat(mind));
            fightConsideration.addAppraisal(new DefenseAppraisals.ThreatFightable(mind));
            reasoner.addConsideration(fightConsideration);

            // FLIGHT of fight-or-flight
            var fleeConsideration = new ThresholdConsideration<Mind>(() => {
                // run away
                var threat = DefenseAppraisals.NearbyThreat.greatestThreat(mind);
                if (threat != null) {
                    // TODO: get multiple threats to find the path to avoid as many as possible
                    // set a task to "avoid" (get out of range of bird)
                    // add avoid task
                    state.setPlan(new[] {new AvoidEntity(mind, threat.Entity, TargetSource.RANGE_MED)});
                }
            }, 0.4f, "flee");
            fleeConsideration.addAppraisal(new DefenseAppraisals.NearbyThreat(mind));
            fleeConsideration.addAppraisal(new DefenseAppraisals.ThreatFightable(mind).inverse());
            reasoner.addConsideration(fleeConsideration);

            var socialConsideration = new ThresholdConsideration<Mind>(() => {
                // socialize - become friends with nearby ducks
                var thresh = SocialAppraisals.NearbyPotentialAllies.opinionThreshold(mind);
                var fren = SocialAppraisals.NearbyPotentialAllies.bestCandidate(mind, thresh);
                // solve a plan to figure out how to interact
                var socialPlanModel = new SocializingBird();
                var solver = new Solver<SocializingBird>();
                // update the model
                socialPlanModel.energyBudget = SocialAppraisals.FriendBudget.budget(mind);
                var feedRange = TargetSource.RANGE_SHORT;
                socialPlanModel.withinDist = (fren.body.pos - mind.me.body.pos).LengthSquared() < feedRange;

                // solve the model and get action plan
                var goalBrownies = 20;
                var next = solver.Next(socialPlanModel,
                    new Goal<SocializingBird>(x => x.brownies > goalBrownies));
                if (next == null) { // no plan could be found
                    return;
                }

                // translate the action plan to tasks
                var newPlan = new List<PlanTask>();
                var feedTime = 10f;
                var goalFeedTime = Time.TotalTime + feedTime;
                var path = next.Path();
                foreach (var node in path) {
                    if (node.matches(nameof(SocializingBird.chase))) {
                        newPlan.Add(
                            new EntityTarget(mind, fren.Entity, Approach.Within, feedRange, goalFeedTime));
                    } else if (node.matches(nameof(SocializingBird.feed))) {
                        newPlan.Add(new PlanFeed(mind, fren.Entity, goalFeedTime));
                    }
                }

                state.setPlan(newPlan);
            }, 0.2f, "social");
            socialConsideration.addAppraisal(new SocialAppraisals.NearbyPotentialAllies(mind));
            socialConsideration.addAppraisal(new SocialAppraisals.Sociability(mind));
            socialConsideration.addAppraisal(new SocialAppraisals.FriendBudget(mind));
            reasoner.addConsideration(socialConsideration);
        }

        private void makePlans() {
            // run the utility ai planner
            var resultTable = reasoner.execute();
            // store plan log
            if (mind.state.lastPlanTable == null) {
                // ReSharper disable once InconsistentlySynchronizedField - it is null
                state.lastPlanTable = resultTable;
            } else {
                lock (mind.state.lastPlanTable) {
                    state.lastPlanTable = resultTable;
                }
            }

            var chosen = reasoner.choose(resultTable); // pick the best-scored option
            chosen.action(); // execute the action
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
                case PhysicalSignals.ShotSignal sig: {
                    var from = sig.gun.Entity.GetComponent<Wing>();
                    if (from != null && from != mind.me) {
                        // run a being shot interaction
                        var interaction = new ShotInteraction(from, sig);
                        interaction.run(mind.soul, from.mind.soul);
                    }

                    break;
                }
                case PhysicalSignals.BumpSignal sig: {
                    var from = sig.wing;
                    if (from != null && from != mind.me) {
                        // run an interaction for being bumped
                        var interaction = new BumpInteraction(sig);
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
                    // subtract diag hitbox
                    var hitboxRadSq = wing.hitbox.Width * wing.hitbox.Width + wing.hitbox.Height * wing.hitbox.Height;
                    var toWingDist = Mathf.Sqrt(toWing.LengthSquared() + hitboxRadSq);
                    if (toWingDist <= NearbyInteraction.triggerRange) {
                        var interaction = new NearbyInteraction(toWingDist);
                        interaction.run(mind.soul, wing.mind.soul);
                    }
                }
            }
        }
    }
}