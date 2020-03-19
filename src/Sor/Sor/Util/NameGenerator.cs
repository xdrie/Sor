using System.Collections.Generic;
using System.IO;
using Nez;

namespace Sor.Util {
    public static class NameGenerator {
        public static List<string> names;
        
        public static void load() {
            names = new List<string>();
            // load name list
            using var nameStream = Core.Content.OpenDataStream("Data/text/short_names.txt");
            using var sr = new StreamReader(nameStream);
            while (!sr.EndOfStream) {
                names.Add(sr.ReadLine());
            }
        }

        public static string next() => names.RandomItem();
    }
}