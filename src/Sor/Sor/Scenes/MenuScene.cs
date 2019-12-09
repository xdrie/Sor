using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Scenes {
    public class MenuScene : BaseGameScene {
        public override void Initialize() {
            base.Initialize();

            ClearColor = gameContext.assets.bgColor;

            var ui = CreateEntity("ui");
            
            var titleText = new TextComponent(gameContext.assets.font, "SOR", new Vector2(40, 40),
                gameContext.assets.fgColor);
            ui.AddComponent(titleText);
        }
    }
}