using System;
using System.Drawing;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Console;
using Sor.Game;
using Sor.Scenes;

namespace Sor {
    public class NGame : GlintCore {
        public const string GAME_TITLE = "Sor";
        public const string GAME_VERSION = "0.5.11.12327-dev";

        private readonly GameContext gameContext;

        public Point gameResolution = new Point(960, 540);

        public NGame(Config config) : base(config.w, config.h, windowTitle: GAME_TITLE,
            isFullScreen: config.fullscreen) {
            gameContext = new GameContext(config);
            Global.log.writeLine("game instantiated", GlintLogger.LogLevel.Information);
        }

        protected override void Initialize() {
            base.Initialize();

            Window.Title = GAME_TITLE;
            Window.AllowUserResizing = false;

            // register context service
            Services.AddService(typeof(GameContext), gameContext);
            
            // update logger
            Global.log.verbosity = (GlintLogger.LogLevel) gameContext.config.logLevel;
            if (gameContext.config.logFile != null) {
                Global.log.sinks.Add(new GlintLogger.FileSink(gameContext.config.logFile));
            }

            // update rendering options
            var resolutionPolicy = Scene.SceneResolutionPolicy.ShowAllPixelPerfect;
            if (gameContext.config.scaleMode == (int) Config.ScaleMode.Stretch) {
                resolutionPolicy = Scene.SceneResolutionPolicy.BestFit;
                Window.AllowUserResizing = true;
                Global.log.writeLine("stretch scaling enabled", GlintLogger.LogLevel.Warning);
            }
            DefaultSamplerState = SamplerState.PointClamp;
            DebugConsole.RenderScale = gameContext.config.scale;
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