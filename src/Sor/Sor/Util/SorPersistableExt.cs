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
            public double energy;
            public BirdPersonality ply;

            public WingData(Wing wing) {
                name = wing.name;
                energy = wing.core.energy;
                ply = wing.mind.soul.ply;
            }
        }

        public static void writeWing(this IPersistableWriter w, Wing wing) {
            var wd = new WingData(wing);
            w.Write(wd.name);
            w.Write(wd.energy);
            w.Write(wd.ply);
        }

        public static void readWingData(this IPersistableReader r) {
            
        }
    }
}