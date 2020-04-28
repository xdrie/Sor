using Glint;
using Glint.Branding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Sor.Game;
using Sor.Scenes;

namespace Sor {
    public class NGame : RGameBase<GameContext, Config> {
        public NGame(Config config) : base(config, new GameContext(config), new Point(960, 540)) { }

        protected override void Initialize() {
            base.Initialize();

            DefaultSamplerState = SamplerState.PointClamp;

            if (!Headless) {
                // fixed timestep for physics updates
                IsFixedTimeStep = true;
                Graphics.Instance.Batcher.ShouldRoundDestinations = false;
            }

            Scene = new DevLogoScene<GameContext, Config, MenuScene>(
                new DevLogoSprite(Content.LoadTexture("Data/img/devlogo.png"),
                    32, 32),
                context.assets.palettePurple);
        }
    }
}