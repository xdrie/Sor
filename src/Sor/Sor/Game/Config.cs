using Glint.Config;
using Glint.Util;

namespace Sor.Game {
    public class Config : GameConfigBase {
        public bool maxVfx = true; // enable all visual effects
        public bool cheats = false;

        public override void load() {
            base.load();

            pr.bind(ref maxVfx, VIDEO, nameof(maxVfx));

#if DEBUG // only load debug config in debug builds
            pr.bind(ref cheats, DEBUG, nameof(cheats));
#endif
        }
    }
}