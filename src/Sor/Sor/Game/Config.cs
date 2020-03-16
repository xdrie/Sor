using Glint.Config;
using Glint.Util;

namespace Sor.Game {
    public class Config : GameConfigBase {
        public bool maxVfx = true; // enable all visual effects
        
        // debug config
        public bool cheats = false;
        public bool logInteractions = false;
        
        // - internal config
        public const string INTERNAL = "internal";
        public bool cameraLockedRotation = false;
        public int generatedMapSize = 40;
        public bool enableWalls = false;
        public bool spawnBirds = true;

        public override void load() {
            base.load();

            pr.bind(ref maxVfx, VIDEO, rename(nameof(maxVfx)));

#if DEBUG // only load debug config in debug builds
            pr.bind(ref cheats, DEBUG, rename(nameof(cheats)));
            pr.bind(ref logInteractions, DEBUG, rename(nameof(logInteractions)));
#endif
            
#if DEBUG
            // internal options
            pr.bind(ref cameraLockedRotation, INTERNAL, rename(nameof(cameraLockedRotation)));
            pr.bind(ref generatedMapSize, INTERNAL, rename(nameof(generatedMapSize)));
            pr.bind(ref enableWalls, INTERNAL, rename(nameof(enableWalls)));
            pr.bind(ref spawnBirds, INTERNAL, rename(nameof(spawnBirds)));
#endif
        }
    }
}