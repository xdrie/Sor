using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Console;
using Nez.Sprites;

namespace Sor.Scenes {
    public class MenuScene : BaseGameScene {
        private SpriteRenderer bookrowSpriteRenderer;

        public override void Initialize() {
            base.Initialize();

            ClearColor = gameContext.assets.bgColor;

            var ui = CreateEntity("ui");

            // var titleText = new TextComponent(gameContext.assets.font, "SOR", new Vector2(40, 40),
            //     gameContext.assets.fgColor);
            // ui.AddComponent(titleTexSpr);

            var titleTexNt = CreateEntity("title", new Vector2(290f, 160f));
            titleTexNt.AddComponent(new SpriteRenderer(Core.Content.LoadTexture("Data/ui/904.png")));
            titleTexNt.SetLocalScale(4f);

            var playBtn = CreateEntity("play_button", new Vector2(800f, 120f));
            bookrowSpriteRenderer =
                playBtn.AddComponent(new SpriteRenderer(Core.Content.LoadTexture("Data/ui/bookrow.png")));
            playBtn.SetLocalScale(4f);
            var pressToPlayText = ui.AddComponent(new TextComponent(gameContext.assets.font, "press [E]",
                new Vector2(720, 140f), gameContext.assets.fgColor));

            var versionText = ui.AddComponent(new TextComponent(gameContext.assets.font, NGame.GAME_VERSION,
                new Vector2(10, Resolution.Y - 20f), gameContext.assets.fgColor));
        }

        public override void Update() {
            base.Update();

#if DEBUG
            if (!DebugConsole.Instance.IsOpen) {
#endif
                if (Input.IsKeyPressed(Keys.E)) {
                    // tween
                    bookrowSpriteRenderer
                        .TweenColorTo(gameContext.assets.palette[2], 0.2f)
                        .SetCompletionHandler(t => transitionScene(new PlayScene(), 0.5f))
                        .Start();
                }

                if (Input.IsKeyPressed(Keys.Escape)) {
                    // end this scene
                    Core.Exit();
                }
#if DEBUG
            }
#endif
        }
    }
}