using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Glint;
using Glint.Util;
using Ducia.Framework.Utility.Considerations;
using Ducia.Layer1;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Plans;
using Sor.AI.Signals;
using Sor.Components.Input;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Game.Map;

namespace Sor.AI {
    public class DuckMindState : MindState {
        public Wing me;
        public LogicInputController controller;
        
        public ConcurrentBag<Wing> seenWings = new ConcurrentBag<Wing>(); // visible wings
        public ConcurrentBag<Thing> seenThings = new ConcurrentBag<Thing>(); // visible things

        public ConcurrentDictionary<Wing, int>
            opinion = new ConcurrentDictionary<Wing, int>(); // opinions of others

        public ConcurrentQueue<PlanTask> plan = new ConcurrentQueue<PlanTask>();
        public List<StructuralNavigationGraph.Node> navPath = new List<StructuralNavigationGraph.Node>();
        public ConcurrentDictionary<string, BoardItem> board = new ConcurrentDictionary<string, BoardItem>();

        public ConcurrentDictionary<Consideration<DuckMind>, float> lastPlanLog =
            new ConcurrentDictionary<Consideration<DuckMind>, float>();

        public struct BoardItem {
            public string value;
            public Color col;
            public string tag;
            public float expireTime;

            public BoardItem(string value, string tag, Color col, float expireTime = 0) {
                this.value = value;
                this.tag = tag;
                this.col = col;
                this.expireTime = expireTime;
            }

            public BoardItem(string value, string tag) : this(value, tag, Color.White) { }

            public static implicit operator BoardItem(string v) => new BoardItem(v, "misc");
        }

        public int getOpinion(Wing wing) {
            if (opinion.TryGetValue(wing, out var val)) {
                return val;
            }

            return 0;
        }

        public int addOpinion(Wing they, int val) {
            var opi = getOpinion(they);
            var res = opi + val;
            opinion[they] = res;
            // if (mind.inspected && NGame.context.config.logInteractions) {
            //     Global.log.trace($"({mind.state.me.name}) added {val} opinion for {they.me.name} (total {res})");
            // }

            return res;
        }

        public bool isPlanValid => plan.Any(x => x.status() == PlanTask.Status.Ongoing);

        /// <summary>
        /// copy new plan to task plan
        /// </summary>
        /// <param name="tasks">new task list</param>
        public void setPlan(IEnumerable<PlanTask> tasks) {
            clearPlan();

            foreach (var task in tasks) {
                plan.Enqueue(task);
            }
        }

        public void clearPlan() {
            while (plan.Count > 0) {
                plan.TryDequeue(out var item);
            }
        }

        public bool hasNavPath => navPath != null && navPath.Count > 0;

        public void setNavPath(List<StructuralNavigationGraph.Node> path) {
            navPath = path;
        }

        public void setBoard(string key, BoardItem item) {
            board[key] = item;
        }

        private void tickBoard() {
            var expiredItemKeys = new List<string>();
            lock (board) {
                foreach (var itemKvp in board) {
                    var item = itemKvp.Value;
                    if (item.expireTime > 0 && Time.DeltaTime > item.expireTime) {
                        expiredItemKeys.Add(itemKvp.Key);
                    }
                }

                foreach (var itemKey in expiredItemKeys) {
                    board.TryRemove(itemKey, out var val);
                }
            }
        }

        public void tick() {
            ticks++;
            if (ticks % 10 == 0) {
                tickBoard();
            }
        }

        public void clearVision() {
            while (!seenWings.IsEmpty) {
                seenWings.TryTake(out var val);
            }

            while (!seenThings.IsEmpty) {
                seenThings.TryTake(out var val);
            }
        }

        public void updatePlanLog(Dictionary<Consideration<DuckMind>, float> resultTable) {
            lastPlanLog.Clear();
            foreach (var item in resultTable) {
                lastPlanLog.TryAdd(item.Key, item.Value);
            }
        }
    }
}