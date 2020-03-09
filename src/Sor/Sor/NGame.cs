using Glint;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Sor.Game;
using Sor.Scenes;

namespace Sor {
    public class NGame : RGameBase<Config, GameContext> {
        public const string GAME_TITLE = "Sor";
        public const string GAME_VERSION = "0.6.13.09-dev";

        public static GameContext context => Core.Services.GetService<GameContext>(); 

        public NGame(Config config) : base(config, new GameContext(config), GAME_TITLE, new Point(960, 540)) { }

        protected override void Initialize() {
            base.Initialize();

            DefaultSamplerState = SamplerState.PointClamp;

            // fixed timestep for physics updates
            IsFixedTimeStep = true;

            Scene = new IntroScene();
        }
    }
}