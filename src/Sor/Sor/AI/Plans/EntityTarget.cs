using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Plans {
    public class EntityTarget : TargetSource {
        public readonly Entity nt;

        public EntityTarget(Entity nt, Approach approach = Approach.Precise, float approachRange = RANGE_DIRECT,
            float before = 0) : base(approach, approachRange, before) {
            this.nt = nt;
        }

        public override bool valid() {
            var val = base.valid();
            return val && nt != null;
        }

        public override Vector2 getPosition() => nt.Position;
    }
}