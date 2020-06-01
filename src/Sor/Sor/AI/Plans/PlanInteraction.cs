using Nez;

namespace Sor.AI.Plans {
    public abstract class PlanInteraction : PlanTask {
        public Entity[] interactees;
        private bool done = false;

        public PlanInteraction(DuckMind mind, Entity[] interactees, float before = 0) : base(mind, before) {
            this.interactees = interactees;
        }
        
        public override Status status() {
            // TODO: validity checking on interactions
            if (base.status() == Status.Failed) return Status.Failed; // check base conditions
            foreach (var nt in interactees) {
                if (!nt.Attached) return Status.Failed; // an entity is no longer available
            }
            if (done) return Status.Complete;
            return Status.Ongoing;
        }
        
        public void markDone() {
            done = true;
        }
    }
}