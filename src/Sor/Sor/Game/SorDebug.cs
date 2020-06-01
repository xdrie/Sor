using System.Linq;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Console;
using Sor.AI;
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
        public static void Energy(float val, string name) {
            if (name == null) name = play.state.player.name;
            var wing = play.state.findAllWings().SingleOrDefault(x => x.name == name);
            if (wing == null) {
                debugLog($"no wing named {name} was found");
                return;
            }

            wing.core.energy += val;
            debugLog($"gave {val} energy to {name}");
        }

        [Command("g_class", "changes player wing class")]
        public static void Class(int newClass) {
            var val = (Wing.WingClass) newClass;
            play.state.player.changeClass(val, true);
            debugLog($"changed player class to {val}");
        }

        [Command("g_list", "lists all wings")]
        public static void List() {
            var wings = play.state.findAllWings().ToList();
            var nearbyWings = play.state.findAllWings().Where(x =>
                    x != play.state.player &&
                    ((x.body.pos - play.state.player.body.pos).LengthSquared() <
                     Constants.DuckMind.SENSE_RANGE * Constants.DuckMind.SENSE_RANGE))
                .ToList();
            debugLog(
                $"{wings.Count} wings ({nearbyWings.Count} nearby): {string.Join(",", wings.Select(x => x.name))}");
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
            var wing = play.state.createNpcWing(name,
                play.state.player.Entity.Position + spawnOffset, wingPly);
            play.AddEntity(wing.Entity);
            debugLog($"spawned 1 entity named {wing.Entity.Name}");
        }

        [Command("d_aitrace", "toggles ai tracing in mind display")]
        public static void AiTrace() {
            aiTrace = !aiTrace;
        }

        public static void debugLog(string v) {
            DebugConsole.Instance.Log(v);
            Global.log.info(v);
        }
    }
#endif
}