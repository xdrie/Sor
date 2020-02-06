using Nez;
using Sor.Components.Units;

namespace Sor.Systems {
    public class WingUpdateSystem : EntityProcessingSystem {
        public WingUpdateSystem() : base(new Matcher().All(typeof(WingBody))) { }

        public override void Process(Entity entity) {
            // misc wing updates
            var me = entity.GetComponent<Wing>();
            // calculate metabolic rate of bird
            
        }
    }
}