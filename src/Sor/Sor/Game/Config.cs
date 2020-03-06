using Glint.Config;
using Glint.Util;

namespace Sor.Game {
    public class Config : GameConfigBase {
        public bool maxVfx = true; // enable all visual effects
        public bool cheats = false;
        
        // - internal config
        public const string INTERNAL = "internal";
        public bool cameraLockedRotation = false;

        public override void load() {
            base.load();

            pr.bind(ref maxVfx, VIDEO, rename(nameof(maxVfx)));

#if DEBUG // only load debug config in debug builds
            pr.bind(ref cheats, DEBUG, rename(nameof(cheats)));
#endif
            
#if DEBUG
            // internal options
            pr.bind(ref cameraLockedRotation, INTERNAL, rename(nameof(cameraLockedRotation)));
#endif
        }
    }
}