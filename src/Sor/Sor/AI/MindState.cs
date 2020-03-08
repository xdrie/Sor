using System.Collections.Concurrent;
using System.Collections.Generic;
using Glint;
using Glint.Util;
using LunchLib.AI.Utility.Considerations;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Model;
using Sor.AI.Signals;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Game;
using Sor.Game.Map;

namespace Sor.AI {
    public class MindState {
        public Mind mind;

        public MindState(Mind mind) {
            this.mind = mind;
        }

        public List<Wing> seenWings = new List<Wing>(); // visible wings
        public List<Thing> seenThings = new List<Thing>(); // visible things
        public ConcurrentQueue<MindSignal> signalQueue = new ConcurrentQueue<MindSignal>(); // signals to be processed
        public ConcurrentDictionary<Mind, int> opinion = new ConcurrentDictionary<Mind, int>(); // opinions of others
        public Dictionary<Consideration<Mind>, float> lastPlanTable;
        public Queue<PlanTask> plan = new Queue<PlanTask>();
        public List<StructuralNavigationGraph.Node> navPath = new List<StructuralNavigationGraph.Node>();
        public Dictionary<string, BoardItem> board = new Dictionary<string, BoardItem>();

        public struct BoardItem {
            public string value;
            public Color col;
            public string tag;

            public BoardItem(string value, string tag, Color col) {
                this.value = value;
                this.tag = tag;
                this.col = col;
            }

            public BoardItem(string value, string tag) : this(value, tag, Color.White) { }

            public static implicit operator BoardItem(string v) => new BoardItem(v, "misc");
        }

        public int getOpinion(Mind mind) {
            if (opinion.TryGetValue(mind, out var val)) {
                return val;
            }

            return 0;
        }

        public int addOpinion(Mind they, int val) {
            var opi = getOpinion(they);
            var res = opi + val;
            opinion[they] = res;
            if (mind.debug) {
                Global.log.writeLine($"({mind.me.name}) added {val} opinion for {they.me.name} (total {res})",
                    GlintLogger.LogLevel.Trace);
            }

            return res;
        }
    }
}