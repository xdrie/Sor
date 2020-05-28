using Glint;
using Glint.Components.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Console;
using Nez.Tweens;
using Sor.Components.Input;
using Sor.Components.Inspect;
using Sor.Components.UI;
using Sor.Components.Units;
using Sor.Game;
using Sor.Game.Save;
using Sor.Systems;

namespace Sor.Scenes {
    public class PlayScene : GameScene {
        private const int renderlayer_above = -256;
        private const int renderlayer_map = 512;
        private const int renderlayer_overlay = 1 << 30;

        public bool showingHelp;
        public const float showHelpTime = 4f;

        public PlayState state;

        public PlayScene(PlayState state) {
            this.state = state;
            Core.Services.AddService(state);
            state.scene = this;
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
                AddRenderer(new ScreenSpaceRenderer(1023, renderlayer_overlay));
            fixedRenderer.ShouldDebugRender = false;

            // update main renderer
            mainRenderer.RenderLayers.AddRange(new[] {renderlayer_map, renderlayer_above});
        }

        public override void OnStart() {
            base.OnStart();

            // - scene setup
            // set up map
            AddEntity(state.mapNt);
            gameContext.map = state.mapLoader.mapRepr; // copy map representation
            var mapRenderer = FindEntity("map").GetComponent<TiledMapRenderer>();
            mapRenderer.RenderLayer = renderlayer_map;

            // attach all wings
            foreach (var wing in state.wings) {
                AddEntity(wing.Entity);
                wing.animator.RenderLayer = renderlayer_above;
            }

            state.wings.Clear();

            foreach (var thing in state.things) {
                // attach all things
                AddEntity(thing.Entity);
            }

            state.things.Clear();

            var status = state.rehydrated ? "rehydrated" : "freshly created";
            Global.log.info($"play scene {status}");

            // - hud
            const int hudPadding = 8;
            var statusBarSize = new Point(96, 12);
            var hud = CreateEntity("hud", new Vector2(DesignResolution.X - statusBarSize.X - hudPadding, hudPadding));
            var energyIndicator = hud.AddComponent(new EnergyIndicator());
            energyIndicator.spriteRenderer.RenderLayer = renderlayer_overlay;
            energyIndicator.backdropRenderer.RenderLayer = renderlayer_overlay;

            var notifMsgNt = CreateEntity("notif", new Vector2(24f, 24f));
            var notifyMsg = notifMsgNt.AddComponent(new TextComponent(gameContext.assets.font, "welcome", Vector2.Zero,
                gameContext.assets.fgColor));
            notifyMsg.RenderLayer = renderlayer_overlay;
            var tw = notifyMsg.TweenColorTo(Color.Transparent, 0.4f)
                .SetEaseType(EaseType.CubicIn).SetDelay(1f)
                .SetCompletionHandler(_ => {
                    if (notifMsgNt.Scene != null) notifMsgNt.Destroy();
                });
            tw.Start();

            var hudSystem = AddEntityProcessor(new HudSystem(state.player, hud));
            var wingInteractions = AddEntityProcessor(new WingUpdateSystem());
            var pipsSystem = AddEntityProcessor(new PipsSystem(state.player));

            // add component to make Camera follow the player
            
            // var cameraLockMode = LockedCamera.LockMode.Position;
            // if (NGame.config.cameraLockedRotation) {
            //     cameraLockMode |= LockedCamera.LockMode.Rotation;
            // }
            var followCamera =
                // Camera.Entity.AddComponent(new LockedCamera(player.Entity, Camera, cameraLockMode));
                Camera.Entity.AddComponent(
                    new FollowCamera(state.player.Entity, FollowCamera.CameraStyle.LockOn));
            followCamera.FollowLerp = 0.3f;
            followCamera.RoundPosition = false;
            Camera.AddComponent<CameraShake>();
            Camera.SetMaximumZoom(2f);
            Camera.SetMinimumZoom(0.5f);
            // Camera.SetZoom(-1f);

#if DEBUG
            // draw nav graph (only visible in debug render)
            var navGraphDisplay = CreateEntity("navgraph_display");
            navGraphDisplay.AddComponent(new NavGraphDisplay(mapRenderer));
#endif
        }

        public override void Update() {
            base.Update();

#if DEBUG
            if (!DebugConsole.Instance.IsOpen) {
#endif

                if (Input.IsKeyPressed(Keys.Escape)) {
                    // save the game
                    saveGame();
                    // end this scene
                    TransitionScene<MenuScene>(0.1f);
                }

                if (InputUtils.IsControlDown()) {
                    Core.Instance.IsMouseVisible = true;
                }
                else {
                    Core.Instance.IsMouseVisible = false;
                }

                void removeInspectors() {
                    foreach (var birdNt in FindEntitiesWithTag(Constants.Tags.WING)) {
                        if (birdNt.HasComponent<MindDisplay>()) {
                            birdNt.RemoveComponent<MindDisplay>(); // remove any existing inspectors
                        }
                    }
                }

                if (Input.RightMouseButtonPressed) {
                    removeInspectors();
                }

                // attach inspector
                if (Input.LeftMouseButtonPressed) {
                    removeInspectors(); // remove any existing inspector
                    // find the nearest non-player bird and inspect
                    var nearest = default(Wing);
                    var nearestDist = double.MaxValue;
                    foreach (var birdNt in FindEntitiesWithTag(Constants.Tags.WING)) {
                        var wing = birdNt.GetComponent<Wing>();
                        if (birdNt.HasComponent<PlayerInputController>())
                            continue;

                        var mouseWp = Camera.ScreenToWorldPoint(Input.MousePosition);
                        var distSq = (wing.body.pos - mouseWp).LengthSquared();
                        if (distSq < nearestDist) {
                            nearest = wing;
                            nearestDist = distSq;
                        }
                    }

                    if (nearest != null) {
                        Global.log.info($"selected mind_inspect on {nearest.name}");
                        nearest?.AddComponent(new MindDisplay(state.player, true));
                    }
                }

                // camera zoom
                if (Input.IsKeyPressed(Keys.D0)) {
                    Camera.RawZoom = 1f;
                }

                var zoomDelta = 1f * Time.DeltaTime;
                if (Input.IsKeyDown(Keys.OemPlus)) {
                    Camera.ZoomIn(zoomDelta);
                }

                if (Input.IsKeyDown(Keys.OemMinus)) {
                    Camera.ZoomOut(zoomDelta);
                }
#if DEBUG
            }
#endif
        }

        public override void Unload() {
            base.Unload();

            // unload play context
            Core.Services.RemoveService(typeof(PlayState));

#if DEBUG
            SorDebug.play = null;
#endif
        }

        public void saveGame() {
            var store = gameContext.data.getStore();
            if (gameContext.config.persist) {
                // save the play context
                store.Save(Constants.Game.GAME_SLOT_0, new PlayPersistable(state));
                // TODO: save the experience, etc. data in another persistable
            }
        }
    }
}