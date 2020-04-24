using System;
using System.Collections.Generic;
using Glint;
using Glint.Diagnostics;
using Glint.Platform;
using Sor.Game.Map.Gen;
using Sor.Test;

namespace SorDk {
#if DEBUG
    public static class Maim {
        public static void install(MaimPrompt prompt) {
            prompt.register("mapgen_tests", mapGeneratorTests);
            prompt.register("render_test", renderTest);
        }

        private static void renderTest(string[] obj) {
            using (var game = new TestGame()) {
                game.Run();
            }
        }

        public static void mapGeneratorTests(string[] args) {
            var mapSize = 16;
            var gen = new MapGenerator(mapSize, mapSize, Nez.Random.RNG.Next(int.MinValue, int.MaxValue));
            gen.generate();
            // debug print the grid
            Console.WriteLine("--grid");
            Console.WriteLine(gen.dumpGrid());

            // debug print the rect list
            // Console.WriteLine("--rectlist");
        }
    }
#endif
}