using System.Linq;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Console;
using Sor.AI.Cogs;
using Sor.Components.Units;
using Sor.Scenes;

namespace Sor.Game {
#if DEBUG
    public static class SorDebug {
        public static PlayScene play;
        
        // - debug display vars
        public static bool aiTrace = false;

        [Command("g_energy", "adds energy to the player")]
        public static void Energy(float val) {
            play.playContext.playerWing.core.energy += val;
            debugLog($"gave {val} energy to player");
        }

        [Command("g_class", "changes player wing class")]
        public static void Class(int newClass) {
            var val = (Wing.WingClass) newClass;
            play.playContext.playerWing.changeClass(val);
            debugLog($"changed player class to {val}");
        }

        [Command("g_list", "lists all wings")]
        public static void List() {
            var wings = play.playContext.wings.ToList();
            debugLog($"{wings.Count} wings: {string.Join(",", wings.Select(x => x.name))}");
        }

        [Command("g_kill", "kills a wing")]
        public static void Kill(string name) {
            var wingNt = play.Entities.FindEntity(name);
            wingNt.Destroy();
            debugLog($"killed 1 entity named {wingNt.Name}");
        }

        [Command("g_spawn", "spawns a wing")]
        public static void Spawn(string name, float a = 0f, float s = 0f) {
            var wingPly = new BirdPersonality {A = a, S = s};
            // wingPly.generateRandom();
            var spawnOffset = Vector2Ext.Rotate(new Vector2(0, -120f), Random.NextFloat() * Mathf.PI * 2f);
            var wing = play.playContext.createWing(name, 
                play.playContext.playerWing.Entity.Position + spawnOffset, wingPly);
            play.AddEntity(wing.Entity);
            debugLog($"spawned 1 entity named {wing.Entity.Name}");
        }

        [Command("d_aitrace", "toggles ai tracing in mind display")]
        public static void AiTrace() {
            aiTrace = !aiTrace;
        }

        public static void debugLog(string v) {
            DebugConsole.Instance.Log(v);
            Global.log.writeLine(v, GlintLogger.LogLevel.Information);
        }
    }
#endif
}