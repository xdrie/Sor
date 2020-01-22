using Nez;
using Sor.Components.Units;

namespace Sor.Systems {
    public class WingInteractionSystem : EntityProcessingSystem {
        public WingInteractionSystem() : base(new Matcher().All(typeof(WingBody))) { }

        public override void Process(Entity entity) {
            // TODO: why does this exist??
            var me = entity.GetComponent<Wing>();
        }
    }
}