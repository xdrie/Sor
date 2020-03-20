using System;
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
        public bool align = false;

        public const float AT_ANGLE = 0.05f * Mathf.PI;
        public const float AT_POSITION_SQ = 2f * 2f;
        public const float NEAR_POSITION_SQ = 60f * 60f;

        public const float RANGE_DIRECT = 0f;
        public const float RANGE_CLOSE = 40f;
        public const float RANGE_SHORT = 80f;
        public const float RANGE_MED = 150f;
        public const float RANGE_LONG = 400f;

        public TargetSource(Mind mind, Approach approach, float approachRange, float reachBefore) : base(mind,
            reachBefore) {
            this.approach = approach;
            this.approachRange = approachRange;
        }

        public Vector2 approachPosition() {
            var pos = getPosition();
            // don't adjust precise approaches
            if (approach == Approach.Precise) return pos;

            // figure out the point along the way
            var toFrom = pos - mind.me.body.pos;
            toFrom.Normalize();
            toFrom *= approachRange;
            return pos - toFrom;
        }

        public float getTargetAngle() {
            var dirToTarget = Vector2Ext.Normalize(getPosition() - mind.me.body.pos);
            return dirToTarget.ScreenSpaceAngle();
        }

        public bool closeEnoughPosition() {
            var pos = mind.me.body.pos;
            var actualPos = getPosition();
            var approachPos = approachPosition();
            var approachToFrom = approachPos - pos;
            var actualToFrom = actualPos - pos;
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

        private bool closeEnoughApproach() {
            var positionCloseEnough = closeEnoughPosition(); // check position
            if (positionCloseEnough) {
                if (!align) return true;
                // check alignment
                var remainingAngle = Mathf.DeltaAngleRadians(mind.me.body.stdAngle, getTargetAngle());
                return Math.Abs(remainingAngle) < AT_ANGLE;
            }

            return false;
        }

        public override Status status() {
            var baseStatus = base.status();
            if (baseStatus != Status.Ongoing) return baseStatus;

            if (closeEnoughApproach()) return Status.Complete;
            return Status.Ongoing;
        }

        public abstract Vector2 getPosition();

        public Approach approach { get; }
    }
}