using Glint.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            SpriteRenderer addUiSprite(Texture2D texture, Vector2 cornerOffset) {
                var texSpr = new Sprite(texture);
                var spRen = ui.AddComponent(new SpriteRenderer(texSpr));
                spRen.SetLocalOffset(new Vector2(texSpr.Texture2D.Width / 2f, texSpr.Texture2D.Height / 2f) +
                                     cornerOffset);
                return spRen;
            }

            // - main menu layout
            var designScale = 4;
            
            var frillRen = addUiSprite(frillTex, Vector2.Zero);
            var titleRen = addUiSprite(titleTex, new Vector2(128, 24) * designScale);
            var frameRen = addUiSprite(bordFrameTex, new Vector2(24, 40) * designScale);
            var bordWhRen = addUiSprite(bordWhTex, new Vector2(24, 40) * designScale);
            bordWhRen.Color = gameContext.assets.paletteBrown;
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