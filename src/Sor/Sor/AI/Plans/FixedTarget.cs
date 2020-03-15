using Microsoft.Xna.Framework;

namespace Sor.AI.Plans {
    public class FixedTarget : TargetSource {
        private readonly Vector2 pos;

        public FixedTarget(Vector2 pos, Approach approach = Approach.Precise, float approachRange = RANGE_DIRECT,
            float before = 0) : base(approach, approachRange, before) {
            this.pos = pos;
        }

        public override Vector2 getPosition() => pos;
    }
}