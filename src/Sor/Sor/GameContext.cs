using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using Sor.Game;

namespace Sor {
    public class GameContext {
        public Assets assets = new Assets();
        public Config config;
        public GameData data = new GameData();

        public GameContext(Config config) {
            this.config = config;
        }

        public class Assets {
            public BitmapFont font;

            public Color[] palette = {
                new Color(0xede5ce),
                new Color(0x8e9c9d),
                new Color(0xaa5c56),
                new Color(0x887163),
                new Color(0x2f2732),
            };

            public Color colGreen = new Color(137, 202, 143);
            public Color colBlue = new Color(98, 161, 179);
            public Color colYellow = new Color(190, 175, 91);
            public Color colOrange = new Color(189, 133, 91);
            public Color colRed = new Color(189, 91, 91);

            public Color fgColor => palette[0];
            public Color bgColor => palette[4];
        }

        public void loadContent() {
            assets.font = Core.Content.LoadBitmapFont("Data/fonts/ua_squared.fnt");
        }
    }
}