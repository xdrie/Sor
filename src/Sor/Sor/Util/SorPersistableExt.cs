using Nez.Persistence.Binary;
using Sor.AI.Cogs;
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

        public class WingData {
            public string name;
            public Wing.WingClass wingClass;
            public double energy;
            public BirdPersonality ply;

            public WingData() { }

            public WingData(Wing wing) {
                name = wing.name;
                wingClass = wing.wingClass;
                energy = wing.core.energy;
                ply = wing.mind.soul.ply;
            }
        }

        public static void writeWingMeta(this IPersistableWriter w, Wing wing) {
            var wd = new WingData(wing);
            w.Write(wd.name);
            w.Write((int) wd.wingClass);
            w.Write(wd.energy);
            w.Write(wd.ply);
        }

        public static WingData readWingMeta(this IPersistableReader r) {
            var wd = new WingData();
            wd.name = r.ReadString();
            wd.wingClass = (Wing.WingClass) r.ReadInt();
            wd.energy = r.ReadDouble();
            wd.ply = r.ReadPersonality();
            return wd;
        }
    }
}