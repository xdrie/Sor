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
    }
}