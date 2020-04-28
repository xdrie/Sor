using Glint.Sprites;
using Microsoft.Xna.Framework.Graphics;

namespace Sor.Components.Things {
    public class Thing : GAnimatedSprite {
        private static long uidCounter = 0; // uids start at 1
        public long uid = ++uidCounter;
        public Thing(Texture2D texture, int width, int height) : base(texture, width, height) { }
    }
}