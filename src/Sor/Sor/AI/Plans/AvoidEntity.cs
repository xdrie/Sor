using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Plans {
    public class AvoidEntity : EntityTarget {
        public AvoidEntity(Mind mind, Entity nt, Approach approach = Approach.Precise,
            float approachRange = RANGE_DIRECT, float before = 0) : base(mind, nt, approach, approachRange, before) { }

        public override Vector2 getPosition() {
            // we need a position that's far enough away to be safe
            // TODO: calculate a position
            return nt.Position;
        }
    }
}