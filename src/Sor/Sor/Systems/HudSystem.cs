using Nez;
using Sor.Components.UI;
using Sor.Components.Units;

namespace Sor.Systems {
    public class HudSystem : ProcessingSystem {
        private Wing player;
        private IndicatorBar energyIndicator;

        public HudSystem(Wing player, Entity hudNt) {
            this.player = player;
            energyIndicator = hudNt.GetComponent<IndicatorBar>();
        }
        
        public override void Process() {
            // TODO: use better-defined numbers
            energyIndicator.setValue(Mathf.Clamp((float) player.core.ratio, 0, 1));
        }
    }
}