using Ducia.Layer3;
using Nez;

namespace Sor.AI.Plans {
    public class PlanAttack : SingleInteraction<DuckMind> {
        public PlanAttack(DuckMind mind, Entity target, float before = 0) : base(mind, target, before) { }
    }
}