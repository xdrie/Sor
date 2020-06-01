using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Plans {
    public class AvoidEntity : EntityTarget {
        public AvoidEntity(DuckMind mind, Entity nt, float approachRange = RANGE_DIRECT, float before = 0)
            : base(mind, nt, Approach.Precise, approachRange, before) { }

        public override Vector2 getPosition() {
            // we need a position that's far enough away to be safe
            // calculate the vector from them to us, then make sure it's at least the approach distance
            // 1. get dir to me
            var dirToMe = Vector2Ext.Normalize(nt.Position - mind.me.body.pos);
            // 2. scale to minimum range, find resultant (away)
            var targetAway = approachRange * -dirToMe;
            return targetAway;
        }
    }
}