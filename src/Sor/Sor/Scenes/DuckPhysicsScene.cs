using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Sor.AI;
using Sor.Components.Input;
using Sor.Components.Units;
using Sor.Systems;

namespace Sor.Scenes {
    public class DuckPhysicsScene : BaseGameScene {
        private const int renderlayer_ui_overlay = 1 << 30;

        public Entity playerEntity;
        public Wing playerWing;

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
            var duckNt = CreateEntity("physical", new Vector2(300f, 200f));
            duckNt.AddComponent(new Wing(new Mind(null, true)));
            duckNt.AddComponent<LogicInputController>();

            var wingInteractions = AddEntityProcessor(new WingInteractionSystem());
        }

        public override void Update() {
            base.Update();

            if (Input.IsKeyPressed(Keys.Escape)) {
                // end this scene
                transitionScene<MenuScene>(0.1f);
            }

            if (Input.LeftMouseButtonPressed) {
                // set duck target to mouse pos
            }
        }
    }
}