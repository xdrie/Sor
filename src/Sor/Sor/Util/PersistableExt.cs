using Microsoft.Xna.Framework;
using Nez.Persistence.Binary;

namespace Sor.Util {
    public static class PersistableExt {
        public static void Write(this IPersistableWriter w, Vector2 vec2) {
            w.Write(vec2.X);
            w.Write(vec2.Y);
        }

        public static Vector2 ReadVec2(this IPersistableReader r) {
            return new Vector2(r.ReadFloat(), r.ReadFloat());
        }
    }
}