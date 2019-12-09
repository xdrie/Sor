using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Scenes {
    public class BaseGameScene : Scene {
        protected const int renderlayer_background = 256;
        protected const int renderlayer_default = 0;

        protected bool _active = true;

        private readonly Color bgColor = new Color(40, 40, 40);

        protected Renderer mainRenderer;
        protected GameContext gameContext;

        public override void Initialize() {
            base.Initialize();
            
            gameContext = Core.Services.GetService<GameContext>();

            // add a new renderer with renderOrder 0
            mainRenderer = AddRenderer(new RenderLayerRenderer(0, renderlayer_default, renderlayer_background));
            mainRenderer.Camera = Camera;

            ClearColor = bgColor;
        }

        protected void transitionScene<TScene>(float duration = 0.5f) where TScene : BaseGameScene, new() {
            if (_active && !Core.Instance.InScreenTransition) {
                _active = false;
//                Debug.WriteLine($"scene transition to {typeof(TScene).FullName}");
                Core.StartSceneTransition(new CrossFadeTransition(() => new TScene()) {
                    Duration = duration
                });
            }
        }
    }
}