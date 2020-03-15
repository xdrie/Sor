using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Plans {
    public interface ITargetSource {
        Vector2 getPosition();
        Approach approach { get; }
    }

    public enum Approach {
        Precise,
        Within,
    }

    public abstract class TargetSource : PlanTask, ITargetSource {
        public float approachRange = 0;
        
        public const float AT_POSITION_SQ = 2f * 2f;
        public const float NEAR_POSITION_SQ = 60f * 60f;

        public const float RANGE_DIRECT = 0f;
        public const float RANGE_CLOSE = 40f;
        public const float RANGE_SHORT = 80f;
        public const float RANGE_MED = 150f;
        public const float RANGE_LONG = 400f;

        public TargetSource(Mind mind, Approach approach, float approachRange, float reachBefore) : base(mind, reachBefore) {
            this.approach = approach;
            this.approachRange = approachRange;
        }

        public Vector2 approachPosition(Vector2 fromPos) {
            var pos = getPosition();
            // don't adjust precise approaches
            if (approach == Approach.Precise) return pos;

            // figure out the point along the way
            var toFrom = pos - fromPos;
            toFrom.Normalize();
            toFrom *= approachRange;
            return pos - toFrom;
        }
        
        public bool closeEnoughApproach(Vector2 fromPos) {
            var actualPos = getPosition();
            var approachPos = approachPosition(fromPos);
            var approachToFrom = approachPos - fromPos;
            var actualToFrom = actualPos - fromPos;
            switch (approach) {
                case Approach.Precise:
                    return approachToFrom.LengthSquared() < AT_POSITION_SQ;
                case Approach.Within:
                    var myDist = actualToFrom.LengthSquared();
                    var closeEnough = (approachRange * approachRange + NEAR_POSITION_SQ);
                    return myDist < closeEnough;
                default:
                    return false; // never
            }
        }

        public abstract Vector2 getPosition();

        public Approach approach { get; }
    }
}