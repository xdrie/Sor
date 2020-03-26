using Glint;
using Glint.Branding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sor.Game;
using Sor.Scenes;

namespace Sor {
    public class NGame : RGameBase<GameContext, Config> {
        public const string GAME_TITLE = "Sor";
        public const string GAME_VERSION = "0.7.13";

        public NGame(Config config) : base(config, new GameContext(config), GAME_TITLE, new Point(960, 540)) { }

        protected override void Initialize() {
            base.Initialize();

            DefaultSamplerState = SamplerState.PointClamp;

            // fixed timestep for physics updates
            IsFixedTimeStep = true;

            Scene = new DevLogoScene<GameContext, Config, MenuScene>(
                new DevLogoSprite(Content.LoadTexture("Data/img/devlogo.png"),
                    32, 32),
                context.assets.palettePurple);
        }
    }
}