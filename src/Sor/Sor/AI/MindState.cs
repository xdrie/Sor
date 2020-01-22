using System.Collections.Generic;
using Nez;
using Sor.Components.Things;
using Sor.Components.Units;

namespace Sor.AI {
    public class MindState {
        public MindState(Mind mind) {
            // TODO: do this
        }

        public List<Wing> seenWings = new List<Wing>();
        public List<Thing> seenThings = new List<Thing>();
    }
}