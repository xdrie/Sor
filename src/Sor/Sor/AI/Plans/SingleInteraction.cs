using Nez;

namespace Sor.AI.Plans {
    public class SingleInteraction : PlanInteraction {
        public Entity target;

        public SingleInteraction(Mind mind, Entity target, float before = 0) : base(mind, new[] {target}, before) {
            this.target = target;
        }
    }
}