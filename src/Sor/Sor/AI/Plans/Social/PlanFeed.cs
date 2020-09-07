using Ducia;
using Ducia.Game;
using Nez;

namespace Sor.AI.Plans {
    public class PlanFeed : SingleInteraction<DuckMind> {
        public PlanFeed(DuckMind mind, Entity feedTarget, float before = 0) : base(mind, feedTarget, before) { }
    }
}