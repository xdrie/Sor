using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Plans {
    public class EntityTarget : TargetSource {
        public readonly Entity nt;

        public EntityTarget(Mind mind, Entity nt, Approach approach = Approach.Precise, float approachRange = RANGE_DIRECT,
            float before = 0) : base(mind, approach, approachRange, before) {
            this.nt = nt;
        }

        public override Status status() {
            if (base.status() == Status.Failed) return Status.Failed; // check time condition
            if (nt == null) return Status.Failed; // entity must not be null
            return Status.Ongoing;
        }

        public override Vector2 getPosition() => nt.Position;
    }
}