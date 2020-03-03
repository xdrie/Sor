using Glint.Config;
using Glint.Util;

namespace Sor.Game {
    public class Config : GameConfigBase {
        public bool maxVfx = true; // enable all visual effects
        public bool cheats = false;

        public override void load() {
            base.load();
            
            maxVfx = pr.getBool("video.maxVfx", maxVfx);

#if DEBUG // only load debug config in debug builds
            clearSaves = pr.getBool("debug.clearSaves", clearSaves);
            cheats = pr.getBool("debug.cheats", cheats);
#endif
        }
    }
}