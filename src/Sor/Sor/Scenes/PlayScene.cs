using Glint.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Sor.AI;
using Sor.AI.Cogs;
using Sor.Components.Camera;
using Sor.Components.Input;
using Sor.Components.Things;
using Sor.Components.UI;
using Sor.Components.Units;
using Sor.Game;
using Sor.Scenes.Helpers;
using Sor.Systems;

namespace Sor.Scenes {
    public class PlayScene : BaseGameScene {
        private const int renderlayer_backdrop = 65535;
        private const int renderlayer_ui_overlay = 1 << 30;

        public Entity playerEntity;
        public Wing playerWing;

        public override void Initialize() {
            base.Initialize();

            loadGame();
            
            var setup = new PlaySceneSetup(this);
            setup.createFreshGame();

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
            var hud = CreateEntity("hud", new Vector2(Resolution.X - statusBarSize.X - hudPadding, hudPadding));
            var energyIndicator = hud.AddComponent(new IndicatorBar(statusBarSize.X, statusBarSize.Y));
            energyIndicator.setColors(new Color(204, 134, 73), new Color(115, 103, 92));
            energyIndicator.spriteRenderer.RenderLayer = renderlayer_ui_overlay;
            energyIndicator.backdropRenderer.RenderLayer = renderlayer_ui_overlay;

            var hudSystem = AddEntityProcessor(new HudSystem(playerWing, hud));
            var wingInteractions = AddEntityProcessor(new WingInteractionSystem());
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

            if (Input.IsKeyDown(Keys.LeftControl)) {
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

        public void loadGame() {
            var store = gameContext.data.getStore();
            var persistable = new PlayStatePersistable(this);
            store.Load(GameData.TEST_SAVE, persistable);
        }

        public void saveGame() {
            var store = gameContext.data.getStore();
            store.Save(GameData.TEST_SAVE, new PlayStatePersistable(this));
        }
    }
}