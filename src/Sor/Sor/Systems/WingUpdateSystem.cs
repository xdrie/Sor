using Nez;
using Sor.Components.Units;

namespace Sor.Systems {
    public class WingUpdateSystem : EntityProcessingSystem {
        public WingUpdateSystem() : base(new Matcher().All(typeof(WingBody))) { }

        public override void Process(Entity entity) {
            // misc wing updates
            var me = entity.GetComponent<Wing>();
            // TODO: hunger based on wing mass
            // calculate metabolic rate of bird
            me.core.energy *= 0.999f;
        }
    }
}