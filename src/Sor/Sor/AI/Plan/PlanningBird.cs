using Activ.GOAP;

namespace Sor.AI.Plan {
    public class PlanningBird : Agent, Clonable<PlanningBird> {
        public bool isHungry = false;
        Option[] opt; // Caching reduces array alloc overheads

        public Option[] Options()
            => opt ??= new Option[] {eatFood};

        public PlanningBird Allocate() => new PlanningBird();

        public PlanningBird Clone(PlanningBird b) {
            b.isHungry = isHungry;
            return b;
        }
        
        public Cost eatFood() => (isHungry = false, 10);
    }
}