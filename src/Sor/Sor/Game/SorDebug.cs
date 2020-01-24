using Nez.Console;
using Sor.Scenes;

namespace Sor.Game {
#if DEBUG
    public static class SorDebug {
        public static PlayScene play;

        [Command("g_energy", "adds energy to the player")]
        public static void Energy(double val) {
            play.playerWing.core.energy += val;
        }
    }
#endif
}