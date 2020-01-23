using System.Collections.Concurrent;
using System.Collections.Generic;
using Glint.Util;
using Sor.AI.Signals;
using Sor.Components.Things;
using Sor.Components.Units;

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
            Global.log.writeLine($"({mind.me.name}) added {val} opinion for {they.me.name} (total {res})",
                GlintLogger.LogLevel.Trace);
            return res;
        }
    }
}