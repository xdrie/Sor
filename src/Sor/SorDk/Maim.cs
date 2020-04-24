using System;
using System.Collections.Generic;
using Glint;
using Glint.Platform;
using Sor.Game.Map.Gen;
using Sor.Test;

namespace SorDk {
#if DEBUG
    public static class Maim {
        public static void launch(List<string> commands) {
            Console.WriteLine("-- MAIM (MAintenance IMmediate access) mode");
            Console.WriteLine($"run with commands: {string.Join(',', commands)}");
            var commandQueue = new Queue<string>(commands);
            var request = default(string);
            while (true) {
                if (commandQueue.Count > 0) {
                    request = commandQueue.Dequeue();
                }
                else {
                    // interactive requests
                    Console.WriteLine(@"
0. print debug information
1. mapgen tests
q. quit
");
                    request = Console.ReadLine();
                }

                if (string.IsNullOrEmpty(request) || request == "q") {
                    break;
                }

                if (int.TryParse(request, out var choice)) {
                    switch (choice) {
                        case 0:
                            // this only works on desktop
                            var platform = new DesktopPlatform();
                            platform.logSystemInformation(); // print debug info
                            break;
                        case 1:
                            // mapgen tests
                            mapGeneratorTests();
                            break;
                    }
                }
                else if (request == "render_test") {
                    using (var game = new TestGame()) {
                        game.Run();
                    }
                }
            }
        }

        public static void mapGeneratorTests() {
            var mapSize = 16;
            var gen = new MapGenerator(mapSize, mapSize, Nez.Random.RNG.Next(int.MinValue, int.MaxValue));
            gen.generate();
            // debug print the grid
            Console.WriteLine("--grid");
            var sb = gen.dumpGrid();

            // debug print the rect list
            Console.WriteLine("--rectlist");
        }
    }
#endif
}