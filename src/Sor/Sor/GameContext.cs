using Nez;
using Nez.BitmapFonts;

namespace Sor {
    public class GameContext {
        public Assets assets = new Assets();
        
        public class Assets {
            public BitmapFont font;
        }
        
        public void loadContent() {
            assets.font = Core.Content.LoadBitmapFont("Raw/fonts/ua_squared.fnt");
        }
    }
}