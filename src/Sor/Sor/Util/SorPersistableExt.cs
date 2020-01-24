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

        public static void writeWing(this IPersistableWriter w, Wing wing) {
            w.writeFromBody(wing.body);
            w.Write(wing.core.energy);
        }

        public static void readToWing(this IPersistableReader r, Wing wing) {
            r.readToBody(wing.body);
            wing.core.energy = r.ReadFloat();
        }
    }
}