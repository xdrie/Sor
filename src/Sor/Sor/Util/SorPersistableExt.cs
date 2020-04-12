using System;
using Glint.Physics;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence.Binary;
using Sor.AI;
using Sor.AI.Cogs;
using Sor.Components.Items;
using Sor.Components.Units;

namespace Sor.Util {
    public static class SorPersistableExt {
        public class BodyData {
            public Vector2 pos;
            public Vector2 velocity;
            public float angle;
            public float angularVelocity;

            public BodyData() { }

            public BodyData(KinBody body) {
                pos = body.pos;
                velocity = body.velocity;
                angle = body.angle;
                angularVelocity = body.angularVelocity;
            }

            public void copyTo(KinBody body) {
                body.pos = pos;
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
            public float energy;
            public BirdPersonality ply;
            public bool armed;

            public WingData() { }

            public WingData(Wing wing) {
                name = wing.name;
                wingClass = wing.wingClass;
                energy = wing.core.energy;
                ply = wing.mind.soul.ply;
                armed = wing.HasComponent<Shooter>();
            }
        }

        public static void writeWingMeta(this IPersistableWriter w, Wing wing) {
            var wd = new WingData(wing);
            w.Write(wd.name);
            w.Write((int) wd.wingClass);
            w.Write(wd.energy);
            w.writePersonality(wd.ply);
            w.Write(wd.armed);
        }

        public static WingData readWingMeta(this IPersistableReader r) {
            var wd = new WingData();
            wd.name = r.ReadString();
            wd.wingClass = (Wing.WingClass) r.ReadInt();
            wd.energy = r.ReadFloat();
            wd.ply = r.readPersonality();
            wd.armed = r.ReadBool();
            return wd;
        }

        public static void writePersonality(this IPersistableWriter w, BirdPersonality ply) {
            w.Write(ply.A);
            w.Write(ply.S);
        }

        public static BirdPersonality readPersonality(this IPersistableReader r) {
            var ply = new BirdPersonality();
            ply.A = r.ReadFloat();
            ply.S = r.ReadFloat();
            return ply;
        }

        public static void writeWingMemory(this IPersistableWriter w, Mind mind) {
            throw new NotImplementedException();
        }

        public static void readWingMemory(this IPersistableReader r, Mind mind) {
            throw new NotImplementedException();
        }
    }
}