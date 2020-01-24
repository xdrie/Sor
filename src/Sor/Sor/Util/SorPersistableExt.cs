using Nez.Persistence.Binary;
using Sor.Components.Units;

namespace Sor.Util {
    public static class SorPersistableExt {
        public static void writeFromBody(this IPersistableWriter w, KinBody body) {
            w.Write(body.Transform.LocalPosition);
            w.Write(body.velocity);
            w.Write(body.angle);
            w.Write(body.angularVelocity);
        }

        public static void readToBody(this IPersistableReader r, KinBody body) {
            body.Transform.LocalPosition = r.ReadVec2();
            body.velocity = r.ReadVec2();
            body.angle = r.ReadFloat();
            body.angularVelocity = r.ReadFloat();
        }
    }
}