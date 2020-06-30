using Microsoft.Xna.Framework.Input;
using Nez;
using Sor.AI;
using Sor.AI.Cogs;
using Sor.Components.Units;

namespace Sor.Scenes {
    public class TestNavScene : GameScene {
        public override void Initialize() {
            base.Initialize();
            
            // set up a duck
            var duckNt = new Entity();
            var wing = duckNt.AddComponent(new Wing(new DuckMind(new AvianSoul(), true)));
        }

        public override void Update() {
            base.Update();
            
            if (Input.IsKeyPressed(Keys.Escape)) {
                TransitionScene<MenuScene>(0.1f);
            }
        }
    }
}