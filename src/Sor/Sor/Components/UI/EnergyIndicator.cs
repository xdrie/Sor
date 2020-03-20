using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Units;

namespace Sor.Components.UI {
    public class EnergyIndicator : IndicatorBar {
        public EnergyIndicator() : base(96, 12) { }

        public override void Initialize() {
            base.Initialize();

            setColors(new Color(115, 103, 92),
                new Color(204, 134, 73),
                NGame.context.assets.colRed);
        }

        public void refresh(Wing wing) {
            setValue(wing.core.ratio);
        }
    }
}