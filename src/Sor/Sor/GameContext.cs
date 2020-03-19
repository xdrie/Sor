using Glint.Config;
using Glint.Game;
using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using Sor.Game;
using Sor.Game.Map;
using Sor.Util;

namespace Sor {
    public class GameContext : ContextBase<Config> {
        public Assets assets { get; } = new Assets();
        public MapRepr map;

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
            
            public Color colFire1 = new Color(255, 169, 15);
            public Color colFire2 = new Color(224, 108, 70);

            public Color fgColor => paletteWhite;
            public Color bgColor => palettePurple;
            
            public ref Color paletteWhite => ref palette[0];
            public ref Color paletteGray => ref palette[1];
            public ref Color paletteOrange => ref palette[2];
            public ref Color paletteBrown => ref palette[3];
            public ref Color palettePurple => ref palette[4];
        }
        
        public GameContext(Config config) : base(config) { }

        public void loadContent() {
            assets.font = Core.Content.LoadBitmapFont("Data/fonts/ua_squared.fnt");
            NameGenerator.load();
        }
    }
}