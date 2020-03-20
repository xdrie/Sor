using Nez;
using Sor.Components.UI;
using Sor.Components.Units;

namespace Sor.Systems {
    public class HudSystem : ProcessingSystem {
        private Wing player;
        private EnergyIndicator energyIndicator;

        public HudSystem(Wing player, Entity hudNt) {
            this.player = player;
            energyIndicator = hudNt.GetComponent<EnergyIndicator>();
        }

        public override void Process() {
            // update energy indicator
            if (player.core != null) {
                energyIndicator.refresh(player);
            }
        }
    }
}