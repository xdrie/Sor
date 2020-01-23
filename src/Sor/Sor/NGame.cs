using System;
using System.Drawing;
using Glint;
using Nez;
using Sor.Scenes;

namespace Sor {
    public class NGame : GlintCore {
        public const string GAME_TITLE = "Sor";
        public const string GAME_VERSION = "0.5.0.1221-dev";

        private readonly GameContext gameContext;

        public Point gameResolution = new Point(960, 540);

        public NGame(GameContext.Config config) : base(config.w, config.h, windowTitle: GAME_TITLE,
            isFullScreen: config.fullscreen) {
            gameContext = new GameContext(config);
        }

        protected override void Initialize() {
            base.Initialize();

            Window.Title = GAME_TITLE;
            Window.AllowUserResizing = false;

            // Register context service
            Services.AddService(typeof(GameContext), gameContext);

            var resolutionPolicy = Scene.SceneResolutionPolicy.ShowAllPixelPerfect;
            if (gameContext.config.scaleMode == (int) GameContext.Config.ScaleMode.Stretch) {
                resolutionPolicy = Scene.SceneResolutionPolicy.BestFit;
                Window.AllowUserResizing = true;
            }

            Scene.SetDefaultDesignResolution(gameResolution.X, gameResolution.Y, resolutionPolicy);

            // Fixed timestep for physics updates
            IsFixedTimeStep = true;
            TargetElapsedTime =
                TimeSpan.FromSeconds(1d / gameContext.config.framerate); // optional custom framerate

            Scene = new IntroScene();
        }
    }
}