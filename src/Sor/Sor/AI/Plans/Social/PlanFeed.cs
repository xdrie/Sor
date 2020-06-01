using Nez;

namespace Sor.AI.Plans {
    public class PlanFeed : SingleInteraction {
        public PlanFeed(DuckMind mind, Entity feedTarget, float before = 0) : base(mind, feedTarget, before) { }
    }
}