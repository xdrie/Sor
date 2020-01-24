using System;
using System.IO;
using System.Reflection;
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
            var config = new Config();
#if DEBUG
            var defaultConf = Assembly.GetExecutingAssembly().GetManifestResourceStream("SorDk.Res.game.dbg.conf");
#else
            var defaultConf = Assembly.GetExecutingAssembly().GetManifestResourceStream("SorDk.Res.game.conf");
#endif
            if (!File.Exists(conf)) {
                using (var sr = new StreamReader(defaultConf)) {
                    File.WriteAllText(conf, sr.ReadToEnd());
                }
            }

            var confStr = File.ReadAllText(conf);
            config.read(confStr); // load and parse config
            using (var game = new NGame(config)) {
                game.Run();
            }
        }
    }
}