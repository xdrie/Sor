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
        private const int renderlayer_ui_overlay = 1 << 30;

        public override void Initialize() {
            base.Initialize();

            ClearColor = gameContext.assets.bgColor;

            // show cursor
            Core.Instance.IsMouseVisible = true;

            // add fixed renderer
            var fixedRenderer =
                AddRenderer(new ScreenSpaceRenderer(1023, renderlayer_ui_overlay));
            fixedRenderer.ShouldDebugRender = false;

            playerNt = CreateEntity(Constants.Game.PLAYER_NAME, new Vector2(400, 400)).SetTag(Constants.Tags.WING);
            var playerSoul = new AvianSoul();
            playerSoul.ply = BirdPersonality.makeNeutral();
            var playerWing = playerNt.AddComponent(new Wing(new DuckMind(playerSoul, false)));
            playerNt.AddComponent(new PlayerInputController(0));

            // add our physicist
            physicistDuck = CreateEntity("physical", new Vector2(300f, 200f)).SetTag(Constants.Tags.WING);
            var physicistSoul = new AvianSoul {ply = new BirdPersonality {A = 0.8f, S = -0.4f}};
            var duckWing = physicistDuck.AddComponent(new Wing(new DuckMind(physicistSoul, true)));
            physicistDuck.AddComponent<LogicInputController>();
            physicistDuck.AddComponent(new MindDisplay(playerWing, true));
            duckWing.core.energy = 1000f;
            duckWing.mind.state.addOpinion(playerWing, Constants.DuckMind.OPINION_FRIEND);

            // set the player as the target
            duckWing.mind.state.plan.Enqueue(new EntityTarget(duckWing.mind, playerWing.Entity, Approach.Precise, TargetSource.RANGE_MED));

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
            if (Input.LeftMouseButtonDown) {
                // set duck target to mouse pos
                var mouseWp = Camera.ScreenToWorldPoint(Input.MousePosition);
                wing.mind.state.plan.Enqueue(new FixedTarget(wing.mind, mouseWp));
            }
        }
    }
}