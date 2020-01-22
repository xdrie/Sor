using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;

namespace Sor {
    public class GameContext {
        public Assets assets = new Assets();
        public Config config = new Config();

        public class Assets {
            public BitmapFont font;

            public Color[] palette = {
                new Color(0xede5ce),
                new Color(0x8e9c9d),
                new Color(0xaa5c56),
                new Color(0x887163),
                new Color(0x2f2732),
            };

            public Color success = new Color(137, 202, 143);

            public Color fgColor => palette[0];
            public Color bgColor => palette[4];
        }

        public class Config {
            public bool maxVfx = true; // enable all visual effects
        }

        public void loadContent() {
            assets.font = Core.Content.LoadBitmapFont("Data/fonts/ua_squared.fnt");
        }
    }
}