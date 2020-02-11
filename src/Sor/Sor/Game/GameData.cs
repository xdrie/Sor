using System.IO;
using System.Reflection;
using Glint;
using Glint.Util;
using Nez.Persistence.Binary;

namespace Sor.Game {
    public class GameData {
        public static string baseDir;
        public static string SAVE_PATH = "saves";
        public const string TEST_SAVE = "test0.sav";
        private GameContext gx;

        public GameData(GameContext gx) {
            this.gx = gx;
            // initialize data paths
            baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Global.log.writeLine($"base dir is {baseDir}", GlintLogger.LogLevel.Trace);
            SAVE_PATH = Path.Combine(baseDir, SAVE_PATH);
            if (gx.config.clearSaves && Directory.Exists(SAVE_PATH)) {
                Directory.Delete(SAVE_PATH, true);
            }
            if (!Directory.Exists(SAVE_PATH)) {
                Directory.CreateDirectory(SAVE_PATH);
            }
        }

        public FileDataStore getStore() {
            return new FileDataStore(SAVE_PATH, FileDataStore.FileFormat.Text);
        }
    }
}