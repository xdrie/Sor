using Nez;

namespace Sor.AI.Model {
    public abstract class PlanInteraction : PlanTask {
        public Entity[] interactees;

        public PlanInteraction(Entity[] interactees) {
            this.interactees = interactees;
        }
        
        public override bool valid() {
            // TODO: validity checking on interactions
            return true;
        }
    }

    public class PlanFeed : PlanInteraction {
        public Entity feedTarget;

        public PlanFeed(Entity feedTarget) : base(new[] {feedTarget}) {
            this.feedTarget = feedTarget;
        }
    }
}