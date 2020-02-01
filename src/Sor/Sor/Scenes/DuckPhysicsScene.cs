using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Sor.AI;
using Sor.AI.Cogs;
using Sor.Components.Camera;
using Sor.Components.Input;
using Sor.Components.Units;
using Sor.Systems;

namespace Sor.Scenes {
    public class DuckPhysicsScene : BaseGameScene {
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

            // set up scene things
            physicistDuck = CreateEntity("physical", new Vector2(300f, 200f)).SetTag(Constants.ENTITY_WING);
            var duckWing = physicistDuck.AddComponent(new Wing(new Mind(null, true)));
            physicistDuck.AddComponent<LogicInputController>();
            physicistDuck.AddComponent(new MindDisplay(null, true));
            duckWing.core.energy = 1000f;

            playerNt = CreateEntity("player", new Vector2(400, 400)).SetTag(Constants.ENTITY_WING);
            var playerSoul = new AvianSoul(BirdPersonality.makeNeutral());
            playerSoul.calc();
            var playerWing = playerNt.AddComponent(new Wing(new Mind(playerSoul, false)));
            playerNt.AddComponent<PlayerInputController>();

            // set pos to current pos
            duckWing.mind.state.target = physicistDuck.Position;

            var wingInteractions = AddEntityProcessor(new WingInteractionSystem());

            // add component to make Camera follow the player
            var followCamera =
                Camera.Entity.AddComponent(new LockedCamera(playerNt, Camera, LockedCamera.LockMode.Position));
            followCamera.AddComponent<CameraShake>();
        }

        public override void Update() {
            base.Update();

            if (Input.IsKeyPressed(Keys.Escape)) {
                // end this scene
                transitionScene<MenuScene>(0.1f);
            }

            var wing = physicistDuck.GetComponent<Wing>();
            if (Input.LeftMouseButtonDown) {
                // set duck target to mouse pos
                var mouseWp = Camera.ScreenToWorldPoint(Input.MousePosition);
                wing.mind.state.target = mouseWp;
            } else {
                wing.mind.state.target = playerNt.Position;
            }
        }
    }
}