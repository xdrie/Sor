using Glint.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Sor.AI;
using Sor.Components.Camera;
using Sor.Components.Input;
using Sor.Components.UI;
using Sor.Components.Units;
using Sor.Game;
using Sor.Scenes.Helpers;
using Sor.Systems;

namespace Sor.Scenes {
    public class PlayScene : BaseGameScene {
        private const int renderlayer_backdrop = 65535;
        private const int renderlayer_ui_overlay = 1 << 30;

        public bool showingHelp;
        public const float showHelpTime = 4f;

        public Entity playerEntity;
        public Wing playerWing;
        private Entity helpNt;

        public override void Initialize() {
            base.Initialize();

#if DEBUG
            SorDebug.play = this;
#endif

            var setup = new PlaySceneSetup(this);
            setup.createScene();

            ClearColor = gameContext.assets.bgColor;

            // Hide cursor
            Core.Instance.IsMouseVisible = false;

            // add fixed renderer
            var fixedRenderer =
                AddRenderer(new ScreenSpaceRenderer(1023, renderlayer_ui_overlay));
            fixedRenderer.ShouldDebugRender = false;

            // - hud
            const int hudPadding = 8;
            var statusBarSize = new Point(96, 12);
            var hud = CreateEntity("hud", new Vector2(DesignResolution.X - statusBarSize.X - hudPadding, hudPadding));
            var energyIndicator = hud.AddComponent(new IndicatorBar(statusBarSize.X, statusBarSize.Y));
            energyIndicator.setColors(new Color(204, 134, 73), new Color(115, 103, 92));
            energyIndicator.spriteRenderer.RenderLayer = renderlayer_ui_overlay;
            energyIndicator.backdropRenderer.RenderLayer = renderlayer_ui_overlay;

            helpNt = CreateEntity("help");
            var helpDisplay1 = helpNt.AddComponent(new TextComponent(gameContext.assets.font, @"
[IJKL]
[SHIFT]
[2]
",
                new Vector2(140, 140), gameContext.assets.fgColor));
            var helpDisplay2 = helpNt.AddComponent(new TextComponent(gameContext.assets.font, @"
move
boost
capsule
",
                new Vector2(280, 140), gameContext.assets.fgColor));
            helpDisplay1.RenderLayer = renderlayer_ui_overlay;
            helpDisplay2.RenderLayer = renderlayer_ui_overlay;
            helpNt.SetLocalScale(2f);
            showingHelp = true;
            helpDisplay1.TweenColorTo(Color.Transparent).SetDelay(showHelpTime)
                .SetCompletionHandler(_ => showingHelp = false).Start();
            helpDisplay2.TweenColorTo(Color.Transparent).SetDelay(showHelpTime).Start();

            var hudSystem = AddEntityProcessor(new HudSystem(playerWing, hud));
            var wingInteractions = AddEntityProcessor(new WingUpdateSystem());
            var pipsSystem = AddEntityProcessor(new PipsSystem(playerWing));

            // add component to make Camera follow the player
            var followCamera =
                Camera.Entity.AddComponent(new LockedCamera(playerEntity, Camera, LockedCamera.LockMode.Position));
            followCamera.AddComponent<CameraShake>();
        }

        public override void Update() {
            base.Update();

            if (Input.IsKeyPressed(Keys.Escape)) {
                // save the game
                saveGame();
                // end this scene
                transitionScene<MenuScene>(0.1f);
            }

            if (!showingHelp) {
                if (Input.IsKeyDown(Keys.Tab)) {
                    helpNt.Enabled = true;
                    helpNt.GetComponents<TextComponent>().ForEach(x => x.Color = gameContext.assets.fgColor);
                } else {
                    helpNt.Enabled = false;
                }
            }

            if (InputUtils.IsControlDown()) {
                Core.Instance.IsMouseVisible = true;
            } else {
                Core.Instance.IsMouseVisible = false;
            }

            if (Input.LeftMouseButtonPressed) {
                // find the nearest non-player bird and inspect
                var nearest = default(Wing);
                var nearestDist = double.MaxValue;
                foreach (var birdNt in FindEntitiesWithTag(Constants.ENTITY_WING)) {
                    var wing = birdNt.GetComponent<Wing>();
                    if (birdNt.HasComponent<PlayerInputController>())
                        continue;
                    if (birdNt.HasComponent<MindDisplay>()) {
                        birdNt.RemoveComponent<MindDisplay>(); // remove any existing inspectors
                    }

                    var mouseWp = Camera.ScreenToWorldPoint(Input.MousePosition);
                    var distSq = (birdNt.Position - mouseWp).LengthSquared();
                    if (distSq < nearestDist) {
                        nearest = wing;
                        nearestDist = distSq;
                    }
                }

                if (nearest != null) {
                    Global.log.writeLine($"selected mind_inspect on {nearest.name}", GlintLogger.LogLevel.Information);
                    nearest?.AddComponent(new MindDisplay(playerWing, true));
                }
            }
        }

        public override void Unload() {
            base.Unload();

#if DEBUG
            SorDebug.play = null;
#endif
        }

        public void saveGame() {
            var store = gameContext.data.getStore();
            if (!gameContext.config.clearSaves)
                store.Save(GameData.TEST_SAVE, new PlayPersistable(new PlaySceneSetup(this)));
        }
    }
}