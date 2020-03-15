using Nez;

namespace Sor.AI.Plans {
    public class PlanAttack : SingleInteraction {
        public PlanAttack(Mind mind, Entity target, float before = 0) : base(mind, target, before) { }
    }
}