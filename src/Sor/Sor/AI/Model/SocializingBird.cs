using System;
using Activ.GOAP;

namespace Sor.AI.Model {
    public class SocializingBird : ActionPlanningModel<SocializingBird> {
        public float energyBudget = 0;
        public int brownies = 0; // how much rapport we're hoping to gain
        public bool withinDist = false;

        public const int CHASE_COST = 2;
        public const int FEED_COST = 6;

        public override Option[] ActionOptions => new Option[] {chase, feed};

        public override SocializingBird Clone(SocializingBird b) {
            b.cost = cost;

            b.energyBudget = energyBudget;
            b.brownies = brownies;
            b.withinDist = withinDist;

            return b;
        }

        public Cost chase() {
            // chase down/approach the target
            // only valid if target is far away
            if (!withinDist) {
                withinDist = true;
                cost += CHASE_COST;
                brownies += 2;
                return true;
            }

            return false;
        }

        public Cost feed() {
            // feed bean to target
            // only valid if we are close enough and have energy budget
            if (withinDist) {
                cost += FEED_COST;
                brownies += 10;
                return true;
            }

            return false;
        }
    }
}