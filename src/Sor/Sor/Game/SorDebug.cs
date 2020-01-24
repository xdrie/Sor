using Glint.Util;
using Nez.Console;
using Sor.Components.Units;
using Sor.Scenes;

namespace Sor.Game {
#if DEBUG
    public static class SorDebug {
        public static PlayScene play;

        [Command("g_energy", "adds energy to the player")]
        public static void Energy(float val) {
            play.playerWing.core.energy += val;
            Global.log.writeLine($"gave {val} energy to player", GlintLogger.LogLevel.Information);
        }
        
        [Command("g_class", "changes player wing class")]
        public static void Class(int newClass) {
            var val = (Wing.WingClass) newClass;
            play.playerWing.changeClass(val);
            Global.log.writeLine($"changed player class to {val}", GlintLogger.LogLevel.Information);
        }
    }
#endif
}