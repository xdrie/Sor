using System.IO;
using Sor;

namespace SorDk {
    class Program {
        public const string conf = "game.conf";
        static void Main(string[] args) {
            var config = new GameContext.Config();
            if (!File.Exists(conf)) {
                File.WriteAllText(conf, @"
# game config file

[video]
w=960
h=540
fullscreen=false
scaleMode=0
framerate=60
maxVfx=true

[platform]
logLevel=3
");
            }
            var confStr = File.ReadAllText(conf);
            config.read(confStr); // load and parse config
            using (var game = new NGame(config)) {
                game.Run();
            }
        }
    }
}