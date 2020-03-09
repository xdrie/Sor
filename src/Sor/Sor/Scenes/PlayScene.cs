using Glint;
using Glint.Components.Camera;
using Glint.Game;
using Glint.Scenes;
using Glint.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tweens;
using Sor.AI.Cogs;
using Sor.Components.Input;
using Sor.Components.Inspect;
using Sor.Components.UI;
using Sor.Components.Units;
using Sor.Game;
using Sor.Game.Map;
using Sor.Game.Map.Gen;
using Sor.Systems;

namespace Sor.Scenes {
    public class PlayScene : BaseGameScene<GameContext> {
        private const int renderlayer_backdrop = 65535;
        private const int renderlayer_ui_overlay = 1 << 30;

        public bool showingHelp;
        public const float showHelpTime = 4f;

        public PlayContext playContext;

        public PlayScene(PlayContext playContext) {
            this.playContext = playContext;
            playContext.scene = this;
        }

        public override void Initialize() {
            base.Initialize();

#if DEBUG
            SorDebug.play = this;
#endif

            ClearColor = gameContext.assets.bgColor;

            // Hide cursor
            Core.Instance.IsMouseVisible = false;

            // add fixed renderer
            var fixedRenderer =
                AddRenderer(new ScreenSpaceRenderer(1023, renderlayer_ui_overlay));
            fixedRenderer.ShouldDebugRender = false;
        }

        public override void OnStart() {
            base.OnStart();
            
            // - scene setup

            if (!playContext.rehydrated) { // freshly creating the scene
                playContext.createPlayer(new Vector2(200, 200));
                var unoPly = new BirdPersonality();
                unoPly.generateNeutral();
                var uno = playContext.createWing("uno", new Vector2(-140, 320), unoPly);
                uno.changeClass(Wing.WingClass.Predator);
                var frendPly = new BirdPersonality {A = -0.8f, S = 0.7f};
                var frend = playContext.createWing("frend", new Vector2(-140, 20), frendPly);
            }

            AddEntity(playContext.playerWing.Entity);
            foreach (var wing in playContext.createdWings) { // attach all wings
                AddEntity(wing.Entity);
            }
            playContext.createdWings.Clear();
            
            foreach (var thing in playContext.createdThings) { // attach all things
                AddEntity(thing.Entity);
            }
            playContext.createdThings.Clear(); 
            
            // set up map
            AddEntity(playContext.mapNt);
            // gameContext.map = playContext.mapLoader.mapRepr; // copy map representation
            
            var status = playContext.rehydrated ? "rehydrated" : "freshly created";
            Global.log.writeLine($"play scene {status}", GlintLogger.LogLevel.Information);

            // - hud
            const int hudPadding = 8;
            var statusBarSize = new Point(96, 12);
            var hud = CreateEntity("hud", new Vector2(DesignResolution.X - statusBarSize.X - hudPadding, hudPadding));
            var energyIndicator = hud.AddComponent(new IndicatorBar(statusBarSize.X, statusBarSize.Y));
            energyIndicator.setColors(new Color(204, 134, 73), new Color(115, 103, 92));
            energyIndicator.spriteRenderer.RenderLayer = renderlayer_ui_overlay;
            energyIndicator.backdropRenderer.RenderLayer = renderlayer_ui_overlay;

            var notifMsgNt = CreateEntity("notif", new Vector2(24f, 24f));
            var notifyMsg = notifMsgNt.AddComponent(new TextComponent(gameContext.assets.font, "welcome", Vector2.Zero,
                gameContext.assets.fgColor));
            notifyMsg.RenderLayer = renderlayer_ui_overlay;
            var tw = notifyMsg.TweenColorTo(Color.Transparent, 0.4f)
                .SetEaseType(EaseType.CubicIn).SetDelay(1f)
                .SetCompletionHandler(_ => {
                    if (notifMsgNt.Scene != null) notifMsgNt.Destroy();
                });
            tw.Start();

            var hudSystem = AddEntityProcessor(new HudSystem(playContext.playerWing, hud));
            var wingInteractions = AddEntityProcessor(new WingUpdateSystem());
            var pipsSystem = AddEntityProcessor(new PipsSystem(playContext.playerWing));

            // add component to make Camera follow the player
            var cameraLockMode = LockedCamera.LockMode.Position;
            if (gameContext.config.cameraLockedRotation) {
                cameraLockMode |= LockedCamera.LockMode.Rotation;
            }

            var followCamera =
                Camera.Entity.AddComponent(new LockedCamera(playContext.playerWing.Entity, Camera, cameraLockMode));
            followCamera.AddComponent<CameraShake>();
            Camera.SetMaximumZoom(2f);
            Camera.SetMinimumZoom(0.5f);
            // Camera.SetZoom(-1f);
            
#if DEBUG
            // draw nav graph (only visible in debug render)
            var navGraphDisplay = CreateEntity("navgraph_display");
            navGraphDisplay.AddComponent(new NavGraphDisplay(gameContext.map,
                FindEntity("map").GetComponent<TiledMapRenderer>()));
#endif
        }

        public override void Update() {
            base.Update();

            if (Input.IsKeyPressed(Keys.Escape)) {
                // save the game
                saveGame();
                // end this scene
                TransitionScene<MenuScene>(0.1f);
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
                foreach (var birdNt in FindEntitiesWithTag(Constants.Tags.ENTITY_WING)) {
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
                    nearest?.AddComponent(new MindDisplay(playContext.playerWing, true));
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
            if (!gameContext.config.clearData)
                store.Save(GameData.TEST_SAVE, new PlayPersistable(playContext));
        }
    }
}