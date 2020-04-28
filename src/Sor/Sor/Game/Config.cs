using Glint.Config;

namespace Sor.Game {
    public class Config : GameConfig {
        public const string GAME_TITLE = "Sor";
        public const string GAME_VERSION = "0.7.21";
        
        public override string title => GAME_TITLE;
        public override string version => GAME_VERSION;
        
        public bool maxVfx = true; // enable all visual effects
        
        // debug config
        public bool cheats = false;
        public bool logInteractions = false;

        // - internal config
        public const string INTERNAL = "internal";
        public bool cameraLockedRotation = false;
        public int generatedMapSize = 40;
        public bool enableWalls = false;
        public bool spawnBirds = true; // whether all the randomized environmental birds should spawn
        public bool invisible = false;
        public int mindDisplayAhead = 3;
        public bool generateMap = true;
        public bool threadPoolAi = true; 

        public override void load() {
            base.load();

            pr.bind(ref maxVfx, VIDEO, rename(nameof(maxVfx)));

#if DEBUG // only load debug config in debug builds
            pr.bind(ref cheats, DEBUG, rename(nameof(cheats)));
            pr.bind(ref logInteractions, DEBUG, rename(nameof(logInteractions)));
            pr.bind(ref mindDisplayAhead, DEBUG, rename(nameof(mindDisplayAhead)));
#endif
            
#if DEBUG
            // internal options
            pr.bind(ref cameraLockedRotation, INTERNAL, rename(nameof(cameraLockedRotation)));
            pr.bind(ref generatedMapSize, INTERNAL, rename(nameof(generatedMapSize)));
            pr.bind(ref enableWalls, INTERNAL, rename(nameof(enableWalls)));
            pr.bind(ref spawnBirds, INTERNAL, rename(nameof(spawnBirds)));
            pr.bind(ref invisible, INTERNAL, rename(nameof(invisible)));
            pr.bind(ref generateMap, INTERNAL, rename(nameof(generateMap)));
            pr.bind(ref threadPoolAi, INTERNAL, rename(nameof(threadPoolAi)));
#endif
        }
    }
}