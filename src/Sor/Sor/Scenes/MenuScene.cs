using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace Sor.Scenes {
    public class MenuScene : BaseGameScene {
        public override void Initialize() {
            base.Initialize();

            ClearColor = gameContext.assets.bgColor;

            var ui = CreateEntity("ui");
            
            // var titleText = new TextComponent(gameContext.assets.font, "SOR", new Vector2(40, 40),
            //     gameContext.assets.fgColor);
            // ui.AddComponent(titleTexSpr);
            
            var titleTex = Core.Content.Load<Texture2D>("UI/904");
            var titleTexNt = CreateEntity("title", new Vector2(290f, 160f));
            titleTexNt.AddComponent(new SpriteRenderer(titleTex));
            titleTexNt.SetLocalScale(4f);
        }
    }
}