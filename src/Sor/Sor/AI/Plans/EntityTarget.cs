using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Plans {
    public class EntityTarget : TargetSource {
        public readonly Entity nt;

        public EntityTarget(Mind mind, Entity nt, Approach approach = Approach.Precise, float approachRange = RANGE_DIRECT,
            float before = 0) : base(mind, approach, approachRange, before) {
            this.nt = nt;
        }

        public override bool valid() {
            var val = base.valid(); // time condition
            // entity must not be null 
            return val && nt != null;
        }

        public override Vector2 getPosition() => nt.Position;
    }
}