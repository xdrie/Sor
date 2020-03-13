using Nez;

namespace Sor.AI.Plans {
    public abstract class PlanInteraction : PlanTask {
        public Entity[] interactees;

        public PlanInteraction(Entity[] interactees, float before = 0) : base(before) {
            this.interactees = interactees;
        }
        
        public override bool valid() {
            // TODO: validity checking on interactions
            return true;
        }
    }

    public class PlanFeed : PlanInteraction {
        public Entity feedTarget;

        public PlanFeed(Entity feedTarget, float before = 0) : base(new[] {feedTarget}, before) {
            this.feedTarget = feedTarget;
        }
    }
}