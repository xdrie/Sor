using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Plans {
    public class EntityTarget : TargetSource {
        /// <summary>
        /// the target entity
        /// </summary>
        public readonly Entity nt;

        public EntityTarget(DuckMind mind, Entity nt, Approach approach = Approach.Precise, float approachRange = RANGE_DIRECT,
            float before = 0) : base(mind, approach, approachRange, before) {
            this.nt = nt;
        }

        public override Status status() {
            if (nt == null) return Status.Failed; // entity must not be null
            var baseStatus = base.status();
            if (baseStatus != Status.Ongoing) return baseStatus;
            return Status.Ongoing;
        }

        /// <summary>
        /// position of the target entity
        /// </summary>
        /// <returns></returns>
        public override Vector2 getPosition() => nt.Position;
    }
}