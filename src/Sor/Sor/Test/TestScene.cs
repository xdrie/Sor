using Microsoft.Xna.Framework;
using Nez;
using Sor.Game;

namespace Sor.Test {
    public class TestScene : Scene {
        public override void Initialize() {
            base.Initialize();

            ClearColor = Color.RosyBrown;
        }

        public override void OnStart() {
            base.OnStart();

            // test text
            var ui = CreateEntity("ui");
            ui.AddComponent(new TextComponent(Graphics.Instance.BitmapFont,
                $"test text! this is running v{Config.GAME_VERSION}\nabcdefghijklmnopqrstuvwxyz0123456789", new Vector2(20, 20),
                Color.White));
        }
    }
}