using Nez;
using Sor.Game;

namespace Sor.Test {
    public class TestGame : Core {
        public TestGame() : base(960, 540) { }

        protected override void Initialize() {
            base.Initialize();

            Scene = new TestScene();
        }
    }
}