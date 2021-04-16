using Glint.Components.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Sor.AI;
using Sor.AI.Cogs;
using Sor.AI.Plans;
using Sor.Components.Input;
using Sor.Components.Inspect;
using Sor.Components.Units;
using Sor.Game;
using Sor.Systems;

namespace Sor.Scenes {
    public class DuckPhysicsScene : GameScene {
        private Entity physicistDuck;
        private Entity playerNt;
        private const int renderlayer_overlay = 1 << 30;

        public override void Initialize() {
            base.Initialize();

            ClearColor = gameContext.assets.bgColor;

            // show cursor
            Core.Instance.IsMouseVisible = true;

            // add fixed renderer
            var fixedRenderer =
                AddRenderer(new ScreenSpaceRenderer(1023, renderlayer_overlay));
            fixedRenderer.ShouldDebugRender = false;

            playerNt = CreateEntity(Constants.Game.PLAYER_NAME, new Vector2(400, 400)).SetTag(Constants.Tags.WING);
            var playerSoul = new AvianSoul();
            playerSoul.ply = BirdPersonality.makeNeutral();
            var playerWing = playerNt.AddComponent(new Wing(new DuckMind(playerSoul, false)));
            playerNt.AddComponent(new PlayerInputController(0));

            // add our physicist
            physicistDuck = CreateEntity("physical", new Vector2(300f, 200f)).SetTag(Constants.Tags.WING);
            var physicistSoul = new AvianSoul {ply = new BirdPersonality {A = -0.4f, S = 0.7f}};
            var duckWing = physicistDuck.AddComponent(new Wing(new DuckMind(physicistSoul, true)));
            physicistDuck.AddComponent<LogicInputController>();
            var md = physicistDuck.AddComponent(new MindDisplay(playerWing, true));
            md.RenderLayer = renderlayer_overlay;
            duckWing.mind.state.addOpinion(playerWing, Constants.DuckMind.OPINION_FRIEND);

            // give both of them plenty of energy
            playerWing.core.energy = duckWing.core.energy = 40000f;

            // set the player as the target
            duckWing.mind.state.plan.Enqueue(new EntityTarget(duckWing.mind, playerWing.Entity, Approach.Within,
                TargetSource.RANGE_MED));

            var wingInteractions = AddEntityProcessor(new WingUpdateSystem());

            // add component to make Camera follow the player
            var followCamera =
                Camera.Entity.AddComponent(new LockedCamera(playerNt, Camera, LockedCamera.LockMode.Position));
            followCamera.AddComponent<CameraShake>();
        }

        public override void Update() {
            base.Update();

            if (Input.IsKeyPressed(Keys.Escape)) {
                // end this scene
                TransitionScene<MenuScene>(0.1f);
            }

            var wing = physicistDuck.GetComponent<Wing>();
            var wingPlan = wing.mind.state.plan;
            if (Input.LeftMouseButtonPressed ||
                (Input.GamePads.Length > 0 && Input.GamePads[0].IsButtonPressed(Buttons.LeftStick))) {
                // set duck target to mouse pos
                var mouseWp = Camera.ScreenToWorldPoint(Input.MousePosition);
                wingPlan.Enqueue(new FixedTarget(wing.mind, mouseWp, Approach.Within,
                    TargetSource.RANGE_CLOSE));
            }

            if (Input.IsKeyPressed(Keys.OemPeriod) ||
                (Input.GamePads.Length > 0 && Input.GamePads[0].IsButtonPressed(Buttons.DPadDown))) {
                wing.mind.state.clearPlan(); // clear plan
            }

            if (Input.RightMouseButtonPressed ||
                (Input.GamePads.Length > 0 && Input.GamePads[0].IsButtonPressed(Buttons.RightStick))) {
                // set duck target to follow me
                wingPlan.Enqueue(new EntityTarget(wing.mind, playerNt, Approach.Within, TargetSource.RANGE_CLOSE));
            }
        }
    }
}