using System.Collections.Generic;
using System.Linq;
using Activ.GOAP;
using Ducia;
using Microsoft.Xna.Framework;
using MoreLinq;
using Nez;
using Nez.AI.Pathfinding;
using Sor.AI.Consid;
using Sor.AI.Model;
using Sor.AI.Plans;
using Sor.Components.Things;
using Sor.Game.Map;
using Sor.Util;

namespace Sor.AI.Systems {
    public partial class ThinkSystem {
        private void socialAction() {
            // socialize - become friends with nearby ducks
            var thresh = SocialAppraisals.NearbyPotentialAllies.opinionThreshold(mind);
            var fren = SocialAppraisals.NearbyPotentialAllies.bestCandidate(mind, thresh);
            // solve a plan to figure out how to interact
            var socialPlanModel = new SocializingBird();
            var solver = new Solver<SocializingBird>();
            // update the model
            socialPlanModel.energyBudget = SocialAppraisals.FriendBudget.budget(mind);
            var feedRange = TargetSource.RANGE_SHORT;
            socialPlanModel.withinDist = (fren.body.pos - mind.state.me.body.pos).LengthSquared() < feedRange;

            // solve the model and get action plan
            var goalBrownies = 20;
            var next = solver.Next(socialPlanModel, new Goal<SocializingBird>(x => x.brownies > goalBrownies));
            if (next == null) {
                // no plan could be found
                return;
            }

            // translate the action plan to tasks
            var newPlan = new List<PlanTask<DuckMind>>();
            var feedTime = 10f;
            var goalFeedTime = Time.TotalTime + feedTime;
            var path = next.Path();
            foreach (var node in path) {
                if (node.matches(nameof(SocializingBird.chase))) {
                    newPlan.Add(new EntityTarget(mind, fren.Entity, Approach.Within, feedRange, goalFeedTime)
                        {align = true});
                }
                else if (node.matches(nameof(SocializingBird.feed))) {
                    newPlan.Add(new PlanFeed(mind, fren.Entity, goalFeedTime));
                }
            }

            state.setPlan(newPlan);
        }

        private void fleeAction() {
            // run away
            var threat = DefenseAppraisals.NearbyThreat.greatestThreat(mind);
            if (threat != null) {
                // TODO: get multiple threats to find the path to avoid as many as possible
                // set a task to "avoid" (get out of range of bird)
                // add avoid task
                state.setPlan(new[] {new AvoidEntity(mind, threat.Entity, TargetSource.RANGE_MED)});
            }
        }

        private void fightAction() {
            // fight threat nearby
            // TODO: figure out the most "threatening" wing, delegate to goal planner
            var threat = DefenseAppraisals.NearbyThreat.greatestThreat(mind);
            if (threat != null) {
                // TODO: improve fighting/engagement, delegate to action planner
                // follow and attack them
                state.setPlan(new PlanTask<DuckMind>[] {
                    new EntityTarget(mind, threat.Entity, Approach.Within, TargetSource.RANGE_CLOSE) {align = true},
                    new PlanAttack(mind, threat.Entity),
                });
            }
        }

        private void exploreAction() {
            // explore action
            // TODO: a more interesting/useful explore action
            // don't pathfind if we already have a valid path
            if (state.hasNavPath && state.isPlanValid) return;

            // cancel if no map model
            if (NGame.context.map == null) return;

            // attempt to do a room-to-room pathfind
            // attempt to pathfind using the structural navigation graph
            // get the nearest node
            var nearestNode = NGame.context.map.sng.nodes.MinBy(x =>
                    (mind.state.me.body.pos - NGame.context.map.tmxMap.TileToWorldPosition(x.pos.ToVector2()))
                    .LengthSquared())
                .First();
            // get the nearest room
            var nearestRoom = NGame.context.map.roomGraph.rooms.MinBy(x =>
                    (mind.state.me.body.pos - NGame.context.map.tmxMap.TileToWorldPosition(x.center.ToVector2()))
                    .LengthSquared())
                .First();
            // TODO: navigate by room, not by node
            // choose a goal room by randomly walking the graph
            // var goalNode = NGame.context.map.sng.nodes.Single(x => x.room == goalRoom);
            var goalNode = nearestNode;
            var visited = new Dictionary<StructuralNavigationGraph.Node, bool>();
            var walkDist = 6;
            for (var i = 0; i < walkDist; i++) {
                visited[goalNode] = true;
                var validLinks = goalNode.links?.Where(x => !(visited.ContainsKey(x) && visited[x]));
                if (validLinks != null && validLinks.Any()) {
                    goalNode = validLinks.RandomSubset(1).SingleOrDefault();
                }
                else {
                    break; // we could not walk any further
                }
            }

            var foundPath = AStarPathfinder.Search(NGame.context.map.sng, nearestNode, goalNode);
            if (foundPath == null || !foundPath.Any()) {
                mind.state.setBoard("pathfind",
                    new DuckMindState.BoardItem($"FAILED from S: {nearestNode}, E: {goalNode}", "nav", Color.Red));
                return; // pathfind failed
            }

            mind.state.setBoard("pathfind",
                new DuckMindState.BoardItem($"SUCCESS from S: {nearestNode}, E: {goalNode}", "nav", Color.SeaGreen));

            state.setNavPath(foundPath);

            // TODO: actually use map knowledge to explore
            // queue the points of the map

            var newPlan = new List<PlanTask<DuckMind>>();
            foreach (var pathNode in foundPath) {
                var tmapPos = pathNode.pos.ToVector2();
                newPlan.Add(new FixedTarget(mind, NGame.context.map.tmxMap.TileToWorldPosition(tmapPos),
                    Approach.Within, TargetSource.RANGE_DIRECT));
            }

            state.setPlan(newPlan);

            var nextPt = foundPath.First().pos;
            state.setBoard("exp",
                new DuckMindState.BoardItem($"({nextPt.X}, {nextPt.Y} path[{foundPath.Count}])", "path"));
        }

        private void eatAction() {
            // eat action
            var hungryPlanModel = new HungryBird();
            var hungrySolver = new Solver<HungryBird>();
            var seenBeans = state.seenThings.Where(x => x is Capsule).ToList();
            // update the model
            hungryPlanModel.nearbyBeans = seenBeans.Count;

            // TODO: tweak this so it syncs up with the reasoner selecting the objective
            var targetSatiety = state.me.body.metabolicRate * 15f; // 15 seconds of food
            var next = hungrySolver.Next(hungryPlanModel, new Goal<HungryBird>(x => x.satiety > targetSatiety, null));
            if (next == null) {
                // planning failed
                return;
            }

            var newPlan = new List<PlanTask<DuckMind>>();
            var path = next.Path();
            foreach (var node in path) {
                // handle planning based on the node
                var timePerBean = 5f;
                var beanTimeAcc = Time.TotalTime;
                if (node.matches(nameof(HungryBird.eatBean))) {
                    // plan eating the nearest bean
                    // TODO: add the bean to the target entity queue
                    var bean = seenBeans[0];
                    seenBeans.Remove(bean);
                    beanTimeAcc += timePerBean;
                    newPlan.Add(new EntityTarget(mind, bean.Entity, Approach.Precise, beanTimeAcc));
                }
                else if (node.matches(nameof(HungryBird.visitTree))) {
                    // plan to visit the nearest tree
                    // TODO: how is this done?
                }
            }

            state.setPlan(newPlan);
        }
    }
}