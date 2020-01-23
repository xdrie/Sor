using System;
using System.Drawing;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Sor.Scenes;

namespace Sor {
    public class NGame : GlintCore {
        public const string GAME_TITLE = "Sor";
        public const string GAME_VERSION = "0.5.5.12300-dev";

        private readonly GameContext gameContext;

        public Point gameResolution = new Point(960, 540);

        public NGame(GameContext.Config config) : base(config.w, config.h, windowTitle: GAME_TITLE,
            isFullScreen: config.fullscreen) {
            gameContext = new GameContext(config);
            Global.log.writeLine("game instantiated", GlintLogger.LogLevel.Information);
        }

        protected override void Initialize() {
            base.Initialize();

            Window.Title = GAME_TITLE;
            Window.AllowUserResizing = false;
            // update logger
            Global.log = new GlintLogger((GlintLogger.LogLevel) gameContext.config.logLevel);

            // register context service
            Services.AddService(typeof(GameContext), gameContext);

            // update rendering options
            var resolutionPolicy = Scene.SceneResolutionPolicy.ShowAllPixelPerfect;
            if (gameContext.config.scaleMode == (int) GameContext.Config.ScaleMode.Stretch) {
                resolutionPolicy = Scene.SceneResolutionPolicy.BestFit;
                Window.AllowUserResizing = true;
                Global.log.writeLine("stretch scaling enabled", GlintLogger.LogLevel.Warning);
            }
            Core.DefaultSamplerState = SamplerState.PointClamp;

            Scene.SetDefaultDesignResolution(gameResolution.X, gameResolution.Y, resolutionPolicy);

            // Fixed timestep for physics updates
            IsFixedTimeStep = true;
            TargetElapsedTime =
                TimeSpan.FromSeconds(1d / gameContext.config.framerate); // optional custom framerate

            Global.log.writeLine("graphics settings applied", GlintLogger.LogLevel.Information);
            Scene = new IntroScene();
        }
    }
}