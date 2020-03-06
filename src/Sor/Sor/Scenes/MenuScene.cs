using Glint.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Console;
using Nez.Sprites;
using Nez.Textures;

namespace Sor.Scenes {
    public class MenuScene : BaseGameScene<GameContext> {
        public override void Initialize() {
            base.Initialize();

            ClearColor = gameContext.assets.bgColor;

            var ui = CreateEntity("ui");

            // display game version
            var versionText = ui.AddComponent(new TextComponent(gameContext.assets.font, NGame.GAME_VERSION,
                new Vector2(10, DesignResolution.Y - 20f), gameContext.assets.fgColor));

            // load menu part textures
            var frillTex = Content.LoadTexture("Data/ui/menu/frill.png");
            var titleTex = Content.LoadTexture("Data/ui/menu/title.png");
            var bordFrameTex = Content.LoadTexture("Data/ui/menu/bord_frame.png");
            var bordWhTex = Content.LoadTexture("Data/ui/menu/bord_wh.png");

            // - main menu layout
            // frill
            var frillSpr = new Sprite(frillTex);
            var frill = ui.AddComponent(new SpriteRenderer(frillSpr))
                .SetLocalOffset(new Vector2(frillSpr.Texture2D.Width / 2f, frillSpr.Texture2D.Height / 2f));
        }

        public override void Update() {
            base.Update();

#if DEBUG
            if (!DebugConsole.Instance.IsOpen) {
#endif
                if (Input.IsKeyPressed(Keys.E)) {
                    TransitionScene(new PlayScene(), 0.5f);
                }

#if DEBUG
                // debug scenes
                if (Input.IsKeyPressed(Keys.D1)) {
                    // duck physics
                    TransitionScene<DuckPhysicsScene>();
                }
#endif

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