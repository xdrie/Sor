using Nez;
using Sor.Components.Units;

namespace Sor.Systems {
    public class WingUpdateSystem : EntityProcessingSystem {
        public WingUpdateSystem() : base(new Matcher().All(typeof(WingBody))) { }

        public override void Process(Entity entity) {
            // misc wing updates
            var me = entity.GetComponent<Wing>();
            // calculate metabolic rate of bird
            var metabolicRate = Constants.CALORIES_PER_KG * me.body.mass * Time.DeltaTime;
            if (me.core.energy > 0) {
                me.core.energy -= metabolicRate;
            }
        }
    }
}