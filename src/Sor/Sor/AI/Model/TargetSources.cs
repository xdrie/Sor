using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Model {
    public interface ITargetSource {
        Vector2 getPosition();
        Approach approach { get; }
    }

    public enum Approach {
        Direct,
        CloseRange,
        MediumRange,
        LongRange,
    }

    public abstract class TargetSource : PlanTask, ITargetSource {
        public float failureTime = 0f;

        public const float RANGE_CLOSE = 40f;
        public const float RANGE_MED = 150f;
        public const float RANGE_LONG = 400f;

        public TargetSource(Approach approach, float reachBefore) {
            this.approach = approach;
            this.failureTime = reachBefore;
        }
        
        public override bool valid() {
            if (failureTime <= 0) return true;
            return Time.TotalTime < failureTime;
        }

        public Vector2 approachPosition(Vector2 from) {
            var pos = getPosition();
            // depending on the approach type, make it closer
            var approachDist = 0f;
            switch (approach) {
                case Approach.Direct: return pos;
                case Approach.CloseRange:
                    approachDist = RANGE_CLOSE;
                    break;
                case Approach.MediumRange:
                    approachDist = RANGE_MED;
                    break;
                case Approach.LongRange:
                    approachDist = RANGE_LONG;
                    break;
            }
            // figure out the point along the way
            var toFrom = pos - from;
            toFrom.Normalize();
            toFrom *= approachDist;
            return pos - toFrom;
        }

        public abstract Vector2 getPosition();
        
        public Approach approach { get; }
    }

    public class FixedTargetSource : TargetSource {
        private readonly Vector2 pos;

        public FixedTargetSource(Vector2 pos, Approach approach = Approach.Direct, float before = 0) : base(approach, before) {
            this.pos = pos;
        }

        public override Vector2 getPosition() => pos;
    }

    public class EntityTargetSource : TargetSource {
        public readonly Entity nt;

        public EntityTargetSource(Entity nt, Approach approach = Approach.Direct, float before = 0) : base(approach, before) {
            this.nt = nt;
        }

        public override bool valid() {
            var val =  base.valid();
            return val && nt != null;
        }

        public override Vector2 getPosition() => nt.Position;
    }
}