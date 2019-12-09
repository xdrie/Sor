using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;

namespace Sor {
    public class GameContext {
        public Assets assets = new Assets();

        public class Assets {
            public BitmapFont font;

            public Color[] palette = {
                new Color(0xede5ce),
                new Color(0x8e9c9d),
                new Color(0xaa5c56),
                new Color(0x887163),
                new Color(0x2f2732),
            };

            public Color fgColor => palette[0];
            public Color bgColor => palette[4];
        }

        public void loadContent() {
            assets.font = Core.Content.LoadBitmapFont("Raw/fonts/ua_squared.fnt");
        }
    }
}