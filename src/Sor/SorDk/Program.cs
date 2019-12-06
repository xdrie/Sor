using Sor;

namespace SorDk {
    class Program {
        static void Main(string[] args) {
            using (var game = new NGame()) {
                game.Run();
            }
        }
    }
}