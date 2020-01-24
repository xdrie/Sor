using Microsoft.Xna.Framework;
using Nez.Persistence.Binary;
using Sor.AI.Cogs;
using Sor.Components.Units;

namespace Sor.Util {
    public static class SorPersistableExt {
        public class BodyData {
            public Vector2 pos;
            public Vector2 velocity;
            public float angle;
            public float angularVelocity;

            public BodyData() {}
            public BodyData(KinBody body) {
                this.pos = body.Transform.LocalPosition;
                this.velocity = body.velocity;
                this.angle = body.angle;
                this.angularVelocity = body.angularVelocity;
            }

            public void copyTo(KinBody body) {
                body.Transform.LocalPosition = pos;
                body.velocity = velocity;
                body.angle = angle;
                body.angularVelocity = angularVelocity;
            }
        }
        
        public static void writeBody(this IPersistableWriter w, KinBody body) {
            var bodyData = new BodyData(body);
            w.Write(bodyData.pos);
            w.Write(bodyData.velocity);
            w.Write(bodyData.angle);
            w.Write(bodyData.angularVelocity);
        }

        public static BodyData readBodyData(this IPersistableReader r) {
            var bodyData = new BodyData();
            bodyData.pos = r.ReadVec2();
            bodyData.velocity = r.ReadVec2();
            bodyData.angle = r.ReadFloat();
            bodyData.angularVelocity = r.ReadFloat();
            return bodyData;
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