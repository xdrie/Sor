using System;
using System.Drawing;
using Glint;
using Nez;
using Sor.Scenes;

namespace Sor {
    public class NGame : GlintCore {
        public const string GAME_TITLE = "LOSEM";
        public const string GAME_VERSION = "0.3.2.0518-dev";

        private readonly GameContext gameContext;

        public Point gameResolution = new Point(960, 540);

        public NGame() : base(1920, 1080, windowTitle: GAME_TITLE) {
            gameContext = new GameContext();
        }

        protected override void Initialize() {
            base.Initialize();

            Window.Title = GAME_TITLE;
            Window.AllowUserResizing = true;

            // Register context service
            Services.AddService(typeof(GameContext), gameContext);

            var resolutionPolicy = Scene.SceneResolutionPolicy.BestFit;
            Scene.SetDefaultDesignResolution(gameResolution.X, gameResolution.Y, resolutionPolicy);

            // Fixed timestep for physics updates
            IsFixedTimeStep = true;
            TargetElapsedTime =
                TimeSpan.FromSeconds(1f / 60f); // optional custom framerate

            Scene = new IntroScene();
        }
    }
}