using Glint;
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
            debugLog($"gave {val} energy to player");
        }
        
        [Command("g_class", "changes player wing class")]
        public static void Class(int newClass) {
            var val = (Wing.WingClass) newClass;
            play.playerWing.changeClass(val);
            debugLog($"changed player class to {val}");
        }
        
        [Command("g_kill", "kills a wing")]
        public static void Kill(string name) {
            var wingNt = play.Entities.FindEntity(name);
            wingNt.Destroy();
            debugLog($"killed 1 entity named {wingNt.Name}");
        }
        
        [Command("g_spawn", "spawns a wing")]
        public static void Spawn(string name) {
            var wing = play.createWing(name, play.playerWing.Entity.Position);
            debugLog($"spawned 1 entity named {wing.Entity.Name}");
        }

        public static void debugLog(string v) {
            DebugConsole.Instance.Log(v);
            Global.log.writeLine(v, GlintLogger.LogLevel.Information);
        }
    }
#endif
}