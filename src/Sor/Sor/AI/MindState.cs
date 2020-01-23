using System.Collections.Concurrent;
using System.Collections.Generic;
using Sor.AI.Signals;
using Sor.Components.Things;
using Sor.Components.Units;

namespace Sor.AI {
    public class MindState {
        public MindState(Mind mind) {
            // TODO: do this
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

        public int addOpinion(Mind mind, int val) {
            var opi = getOpinion(mind);
            var res = opi + val;
            opinion[mind] = res;
            return res;
        }
    }
}