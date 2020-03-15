using Nez;

namespace Sor.AI.Plans {
    public abstract class PlanInteraction : PlanTask {
        public Entity[] interactees;

        public PlanInteraction(Mind mind, Entity[] interactees, float before = 0) : base(mind, before) {
            this.interactees = interactees;
        }
        
        public override bool valid() {
            // TODO: validity checking on interactions
            return true;
        }
    }
}