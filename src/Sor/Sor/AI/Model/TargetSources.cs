using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Model {
    public interface ITargetSource {
        Vector2 getPosition();
    }

    public abstract class TargetSource : ITargetSource {
        public float failureTime = 0f;

        public TargetSource(float reachBefore) {
            this.failureTime = reachBefore;
        }

        public virtual bool valid() {
            if (failureTime <= 0) return true;
            return Time.TotalTime < failureTime;
        }

        public abstract Vector2 getPosition();
    }

    public class FixedTargetSource : TargetSource {
        private readonly Vector2 pos;

        public FixedTargetSource(Vector2 pos, float before = 0) : base(before) {
            this.pos = pos;
        }

        public override Vector2 getPosition() => pos;
    }

    public class EntityTargetSource : TargetSource {
        private readonly Entity nt;

        public EntityTargetSource(Entity nt, float before = 0) : base(before) {
            this.nt = nt;
        }

        public override bool valid() {
            var val =  base.valid();
            return val && nt != null;
        }

        public override Vector2 getPosition() => nt.Position;
    }
}