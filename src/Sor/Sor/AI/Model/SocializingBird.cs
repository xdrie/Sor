using Activ.GOAP;
using Ducia;

namespace Sor.AI.Model {
    public class SocializingBird : SmartActionPlanningModel<SocializingBird> {
        // - state
        public float energyBudget { get; set; } = 0;
        public int brownies { get; set; } = 0; // how much rapport we're hoping to gain
        public bool withinDist { get; set; } = false;

        // - const
        public const int CHASE_COST = 2;
        public const int FEED_COST = 6;

        protected override Option[] ActionOptions => new Option[] {chase, feed};

        public Cost chase() {
            // chase down/approach the target
            // only valid if target is far away
            if (withinDist) return false;

            withinDist = true;
            brownies += 2;
            return CHASE_COST;
        }

        public Cost feed() {
            // feed bean to target
            // only valid if we are close enough and have energy budget
            if (!withinDist || energyBudget <= 0) return false;

            // TODO: use a proper value for approximate energy cost of feeding (typically upper bound, so we're stingy)
            energyBudget -= 0.1f;
            brownies += 10;
            return FEED_COST;
        }
    }
}