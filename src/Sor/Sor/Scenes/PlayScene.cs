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
using Sor.Systems;

namespace Sor.Scenes {
    public class PlayScene : BaseGameScene {
        private const int renderlayer_backdrop = 65535;
        private const int renderlayer_ui_overlay = 1 << 30;

        public Wing playerWing;

        public override void Initialize() {
            base.Initialize();

            ClearColor = gameContext.assets.bgColor;

            // Hide cursor
            Core.Instance.IsMouseVisible = false;

            // add fixed renderer
            var fixedRenderer =
                AddRenderer(new ScreenSpaceRenderer(1023, renderlayer_ui_overlay));
            fixedRenderer.ShouldDebugRender = false;

            var playerEntity = CreateEntity("player", new Vector2(200, 200));
            playerWing = playerEntity.AddComponent(new Wing());
            var playerSoul = new AvianSoul {ply = BirdPersonality.makeNeutral()};
            playerSoul.calculateTraits();
            var playerMind = playerWing.AddComponent(new Mind(playerSoul, false));
            playerSoul.mind = playerMind; // associate mind with soul
            playerEntity.AddComponent<PlayerInputController>();

            var duckUnoNt = CreateEntity("duck-uno", new Vector2(-140, 320)).SetTag(Constants.ENTITY_WING);
            var duckUno = duckUnoNt.AddComponent(new Predator());
            duckUno.AddComponent<LogicInputController>();
            duckUno.AddComponent<Mind>();
            duckUno.AddComponent(new MindDisplay(playerWing, true));

            var duckDosNt = CreateEntity("duck-dos", new Vector2(-140, 20)).SetTag(Constants.ENTITY_WING);
            var duckDos = duckDosNt.AddComponent(new Wing());
            var duckDosSoul = new AvianSoul {ply = BirdPersonality.makeNeutral()};
            duckDosSoul.calculateTraits();
            var duckDosMind = duckDosNt.AddComponent(new Mind(duckDosSoul, false));
            duckDosSoul.mind = duckDosMind; // associate mind with soul

            var blockNt = CreateEntity("block", new Vector2(140, 140)).SetTag(Constants.ENTITY_WING);
            var blockColl = blockNt.AddComponent(new BoxCollider(-4, -16, 8, 32));

            var mapAsset = Core.Content.LoadTiledMap("Data/maps/test2.tmx");
            var mapEntity = CreateEntity("map");
            var mapRenderer = mapEntity.AddComponent(new TiledMapRenderer(mapAsset, null, false));
            var loader = new MapLoader(this, mapEntity);
            loader.load(mapAsset);

            Global.log.writeLine("play scene created", GlintLogger.LogLevel.Information);

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
                var currentlyInspected = default(Entity);
                foreach (var birdNt in FindEntitiesWithTag(Constants.ENTITY_WING)) {
                    var wing = birdNt.GetComponent<Wing>();
                    if (birdNt.HasComponent<PlayerInputController>())
                        continue;
                    if (birdNt.HasComponent<MindDisplay>()) {
                        currentlyInspected = birdNt;
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
                    currentlyInspected?.RemoveComponent<MindDisplay>();
                    nearest?.AddComponent(new MindDisplay(playerWing, true));
                }
            }
        }
    }
}