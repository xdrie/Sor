using Ducia;
using Ducia.Game;
using Nez;

namespace Sor.AI.Plans {
    public class PlanAttack : SingleInteraction<DuckMind> {
        public PlanAttack(DuckMind mind, Entity target, float before = 0) : base(mind, target, before) { }
    }
}