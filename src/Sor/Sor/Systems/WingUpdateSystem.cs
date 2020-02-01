using Nez;
using Sor.Components.Units;

namespace Sor.Systems {
    public class WingUpdateSystem : EntityProcessingSystem {
        public WingUpdateSystem() : base(new Matcher().All(typeof(WingBody))) { }

        public override void Process(Entity entity) {
            // TODO: why does this exist??
            var me = entity.GetComponent<Wing>();
            // leak energy???
            me.core.energy *= 0.999f;
        }
    }
}