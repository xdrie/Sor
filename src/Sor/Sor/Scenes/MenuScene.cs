using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glint.Composer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Console;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tweens;
using Sor.Components.Input;
using Sor.Components.UI;
using Sor.Game;

namespace Sor.Scenes {
    public class MenuScene : GameScene {
        public override void Initialize() {
            base.Initialize();

            ClearColor = gameContext.assets.bgColor;

            var ui = CreateEntity("ui");

            // display game version
            var versionStr = Config.GAME_VERSION;
            #if DEBUG
            versionStr += " [DEBUG]";
            #endif
            var versionText = ui.AddComponent(new TextComponent(gameContext.assets.font, versionStr,
                new Vector2(10, DesignResolution.Y - 20f), gameContext.assets.fgColor));

            // load menu part textures
            var frillTex = Content.LoadTexture("Data/ui/menu/frill.png");
            var titleTex = Content.LoadTexture("Data/ui/menu/title.png");
            var bordFrameTex = Content.LoadTexture("Data/ui/menu/bord_frame.png");
            var bordWhTex = Content.LoadTexture("Data/ui/menu/bord_wh.png");
            var buttonTex = Content.LoadTexture("Data/ui/menu/button.png");
            var textFlyTex = Content.LoadTexture("Data/ui/menu/tex_fly.png");
            var textEvoTex = Content.LoadTexture("Data/ui/menu/tex_evo.png");
            var textOptTex = Content.LoadTexture("Data/ui/menu/tex_opt.png");
            var waitTex = Content.LoadTexture("Data/ui/menu/wait.png");

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
            var titleRen = addUiSprite(titleTex, new Vector2(128, 20) * designScale);
            var frameRen = addUiSprite(bordFrameTex, new Vector2(24, 40) * designScale);
            var bordWhRen = addUiSprite(bordWhTex, new Vector2(24, 40) * designScale);
            bordWhRen.Color = gameContext.assets.paletteBrown;

            var menuButtons = default(MenuButtonList);
            
            SpriteAnimator addWait() {
                var waitSprs = Sprite.SpritesFromAtlas(waitTex, 32 * designScale, 32 * designScale);
                var waitNt = CreateEntity("wait", DesignResolution.ToVector2() / 2f);
                var anim = waitNt.AddComponent(new SpriteAnimator(waitSprs[0]));
                anim.AddAnimation("load", waitSprs.ToArray());
                return anim;
            }

            void fadeUiSprite(SpriteRenderer ren) {
                ren.fade(Color.Transparent).Start();
            }

            void bordFlash(Action follow = null) {
                var colTw = bordWhRen.TweenColorTo(gameContext.assets.paletteWhite)
                    .SetDuration(0.4f)
                    .SetEaseType(EaseType.QuadOut)
                    .SetCompletionHandler(_ => follow?.Invoke());
                colTw.Start();
            }

            void uiFocus(Action follow = null) {
                fadeUiSprite(frillRen);
                fadeUiSprite(titleRen);
                fadeUiSprite(frameRen);
                fadeUiSprite(versionText);
                menuButtons.active = false;
                menuButtons.applyToRenderers(fadeUiSprite);
                bordFlash(follow);
            }

            // add controller
            ui.AddComponent(new MenuInputController());
            menuButtons = ui.AddComponent(new MenuButtonList(
                new List<MenuButtonList.Item> {
                    new MenuButtonList.Item(new Sprite(textFlyTex), () => {
                        uiFocus(async () => {
                            var wait = addWait();
                            wait.Play("load");
                            fadeUiSprite(bordWhRen);
                            var playSetup = new PlaySetup(); // empty play context
                            // run load game on a worker thread
                            await Task.Run(() => {
                                GameLoader.loadSave(playSetup); // load from save
                                playSetup.load();
                            });
                            fadeUiSprite(wait);
                            var play = new PlayScene(playSetup);
                            TransitionScene(play, 0.5f);
                        });
                    }),
                    new MenuButtonList.Item(new Sprite(textEvoTex), () => {
                        uiFocus();
                    }),
                    new MenuButtonList.Item(new Sprite(textOptTex), () => {
                        bordFlash();
                    }),
                },
                Sprite.SpritesFromAtlas(buttonTex, 320, 64),
                (new Vector2(112, 64) * designScale) + new Vector2(160, 32)
            ));
        }

        public override void Update() {
            base.Update();

#if DEBUG
            if (!DebugConsole.Instance.IsOpen) {
#endif

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