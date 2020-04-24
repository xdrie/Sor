using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Glint;
using Glint.Config;
using Glint.Platform;
using Sor;
using Sor.Game;
using Sor.Test;

#if !DEBUG
using Glint.Util;
#endif

namespace SorDk {
    class Program {
        public const string conf = "game.conf";

        static void Main(string[] args) {
#if CORERT
            DesktopPlatform.setupCoreRTSupport();
#endif

            var banner = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(SorDk)}.Res.banner.txt");
            using (var sr = new StreamReader(banner)) {
                Console.WriteLine(sr.ReadToEnd());
                Console.WriteLine(Config.GAME_VERSION);
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
            var defaultConf = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"{nameof(SorDk)}.Res.game.dbg.conf");
#else
            var defaultConf =
 Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(SorDk)}.Res.game.conf");
#endif
            var configHelper = new ConfigHelper<Config>();
            var confPath = Path.Join(Global.baseDir, conf);
            configHelper.ensureDefaultConfig(confPath, defaultConf);
            var confStr = File.ReadAllText(confPath);
            var config = configHelper.load(confStr, args); // load and parse config
            // run in crash-cradle (only if NOT debug)
#if !DEBUG
            try {
#endif
            using (var game = new NGame(config)) {
                game.Run();
            }
#if !DEBUG
        }
        catch (Exception ex) {
            Global.log.crit($"fatal error: {ex}");
            throw;
        }
#endif
        }
    }
}