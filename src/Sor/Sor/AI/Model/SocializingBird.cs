using System;
using Activ.GOAP;

namespace Sor.AI.Model {
    public class SocializingBird : ActionPlanningModel<SocializingBird> {
        public float energyBudget = 0;
        public int effort = 0;

        public override Option[] ActionOptions => new Option[] {chase, feed};

        public override SocializingBird Clone(SocializingBird b) {
            b.cost = cost;

            b.effort = effort;
            b.energyBudget = energyBudget;
            return b;
        }

        public Cost chase() {
            // chase down/approach the target
            // only valid if target is far away
            throw new NotImplementedException();
        }

        public Cost feed() {
            // feed bean to target
            // only valid if we are close enough and have energy budget
            throw new NotImplementedException();
        }
    }
}