using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Test {
    public class TestScene : Scene {
        public override void Initialize() {
            base.Initialize();

            ClearColor = Color.RosyBrown;
            SetDesignResolution(960, 540, SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(960, 540);
        }

        public override void OnStart() {
            base.OnStart();

            // test text
            var ui = CreateEntity("ui");
            ui.AddComponent(new TextComponent(Graphics.Instance.BitmapFont,
                $"test text! this is running v0.7.19\nabcdefghijklmnopqrstuvwxyz0123456789", new Vector2(20, 20),
                Color.White));
        }
    }
}