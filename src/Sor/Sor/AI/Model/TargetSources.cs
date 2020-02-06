using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI.Model {
    public interface ITargetSource {
        Vector2 getPosition();
    }

    public class FixedTargetSource : ITargetSource {
        private readonly Vector2 pos;

        public FixedTargetSource(Vector2 pos) {
            this.pos = pos;
        }

        public Vector2 getPosition() => pos;
    }

    public class EntityTargetSource : ITargetSource {
        private readonly Entity nt;

        public EntityTargetSource(Entity nt) {
            this.nt = nt;
        }

        public Vector2 getPosition() => nt.Position;
    }
}