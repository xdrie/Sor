using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Glint;
using Glint.Config;
using Glint.Util;
using Sor;
using Sor.Game;

namespace SorDk {
    class Program {
        public const string conf = "game.conf";

        static void Main(string[] args) {
            var banner = Assembly.GetExecutingAssembly().GetManifestResourceStream("SorDk.Res.banner.txt");
            using (var sr = new StreamReader(banner)) {
                Console.WriteLine(sr.ReadToEnd());
                Console.WriteLine(NGame.GAME_VERSION);
#if DEBUG
                Console.WriteLine("[DEBUG] build, debug code paths enabled. maim mode enabled.");
#endif
            }

#if DEBUG
            // check MAIM (MAintenance IMmediate access) mode
            if (args.Length > 0 && args[0] == "maim") {
                Maim.launch(args.Skip(1).ToList());
                return;
            }
#endif

            // load configuration
#if DEBUG
            var defaultConf = Assembly.GetExecutingAssembly().GetManifestResourceStream("SorDk.Res.game.dbg.conf");
#else
            var defaultConf = Assembly.GetExecutingAssembly().GetManifestResourceStream("SorDk.Res.game.conf");
#endif
            var configHelper = new ConfigHelper<Config>();
            configHelper.ensureDefaultConfig(conf, defaultConf);
            var confStr = File.ReadAllText(conf);
            var config = configHelper.load(confStr, args); // load and parse config
            // run in crash-cradle
            try {
                using (var game = new NGame(config)) {
                    game.Run();
                }
            }
            catch (Exception ex) {
                Global.log.writeLine($"fatal error: {ex}", GlintLogger.LogLevel.Critical);
                throw;
            }
        }
    }
}