using System;
using System.IO;
using System.Reflection;
using Glint.Config;
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
            }
            
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
            using (var game = new NGame(config)) {
                game.Run();
            }
        }
    }
}