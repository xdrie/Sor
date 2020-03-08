using Glint.Config;
using Glint.Game;
using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using Sor.Game;
using Sor.Game.Map;

namespace Sor {
    public class GameContext : ContextBase {
        public Assets assets = new Assets();
        public Config config;
        public override GameConfigBase baseConfig => config;
        public GameData data;
        public MapRepr map;

        public GameContext(Config config) {
            this.config = config;
            this.data = new GameData(this);
        }

        public class Assets {
            public BitmapFont font;

            public Color[] palette = {
                new Color(237, 229, 206), // white
                new Color(142, 156, 157), // gray
                new Color(170, 92, 86), // orange
                new Color(136, 113, 99), // brown
                new Color(47, 39, 50), // purple
            };

            public Color colGreen = new Color(137, 202, 143);
            public Color colBlue = new Color(98, 161, 179);
            public Color colYellow = new Color(190, 175, 91);
            public Color colOrange = new Color(189, 133, 91);
            public Color colRed = new Color(189, 91, 91);

            public Color fgColor => paletteWhite;
            public Color bgColor => palettePurple;
            
            public Color paletteWhite => palette[0];
            public Color paletteGray => palette[1];
            public Color paletteOrange => palette[2];
            public Color paletteBrown => palette[3];
            public Color palettePurple => palette[4];
        }

        public void loadContent() {
            assets.font = Core.Content.LoadBitmapFont("Data/fonts/ua_squared.fnt");
        }
    }
}