using System;
using System.Collections.Generic;

namespace SorDk {
#if DEBUG
    public static class Maim {
        public static void launch(List<string> args) {
            Console.WriteLine("-- MAIM (MAintenance IMmediate access) mode");
        }
    }
#endif
}