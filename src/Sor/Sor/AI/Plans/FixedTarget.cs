using Microsoft.Xna.Framework;

namespace Sor.AI.Plans {
    public class FixedTarget : TargetSource {
        private readonly Vector2 pos;

        public FixedTarget(DuckMind mind, Vector2 pos, Approach approach = Approach.Precise, float approachRange = RANGE_DIRECT,
            float before = 0) : base(mind, approach, approachRange, before) {
            this.pos = pos;
        }

        public override Vector2 getPosition() => pos;
    }
}