using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Plans {
    public class AvoidEntity : EntityTarget {
        public AvoidEntity(Entity nt, Approach approach = Approach.Precise, float approachRange = RANGE_DIRECT, float before = 0) : base(nt, approach, approachRange, before) { }

        public override Vector2 getPosition(Mind mind) {
            // we need a position that's far enough away to be safe
            // TODO: calculate a position
            return nt.Position;
        }
    }
}