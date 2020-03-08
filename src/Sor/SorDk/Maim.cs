using System;
using System.Collections.Generic;
using Sor.Game.Map.Gen;

namespace SorDk {
#if DEBUG
    public static class Maim {
        public static void launch(List<string> args) {
            Console.WriteLine("-- MAIM (MAintenance IMmediate access) mode");
            while (true) {
                Console.WriteLine(@"
0. print debug information
1. mapgen tests
q. quit
");
                var inp = Console.ReadLine();
                if (string.IsNullOrEmpty(inp) || inp == "q") {
                    break;
                }

                if (int.TryParse(inp, out var choice)) {
                    switch (choice) {
                        case 0:
                            // print debug info
                            // TODO: system information
                            break;
                        case 1:
                            // mapgen tests
                            mapGeneratorTests();
                            break;
                    }
                }
            }
        }

        public static void mapGeneratorTests() {
            var mapSize = 12;
            var gen = new MapGenerator(mapSize, mapSize);
            gen.generate();
            // debug print the grid
            Console.WriteLine("--grid");
            for (int c = 0; c < mapSize; c++) {
                for (int r = 0; r < mapSize; r++) {
                    var cell = gen.grid[r * mapSize + c];
                    Console.Write($"{cell,4}");
                }

                Console.WriteLine();
            }

            // debug print the rect list
            Console.WriteLine("--rectlist");
        }
    }
#endif
}