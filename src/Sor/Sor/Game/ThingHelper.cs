using Glint.Util;
using Nez;
using Nez.Persistence.Binary;
using Sor.Components.Things;
using Sor.Util;

namespace Sor.Game {
    public static class ThingHelper {
        public enum ThingKind {
            Unknown,
            Capsule,
            Tree
        }

        public static ThingKind classify(Thing thing) {
            if (thing is Capsule) return ThingKind.Capsule;
            if (thing is Tree) return ThingKind.Tree;
            return ThingKind.Unknown;
        }

        public static void saveThing(this IPersistableWriter wr, Thing thing) {
            wr.Write((int) classify(thing));
            switch (thing) {
                case Capsule cap: {
                    // write status
                    wr.Write(cap.acquired);
                    // write body data
                    wr.writeBody(cap.body);
                    // write meta
                    break;
                }
            } 
        }

        public static Thing loadThing(this IPersistableReader rd) {
            var kind = (ThingKind) rd.ReadInt();
            switch (kind) {
                case ThingKind.Unknown:
                    // unrecognized thing
                    Global.log.writeLine("unrecognized thing kind", GlintLogger.LogLevel.Error);
                    return null;
                case ThingKind.Capsule:
                    // don't load acquired capsules
                    var acquired = rd.ReadBool();
                    if (acquired) return null;
                    var cap = new Capsule();
                    // read body
                    var bodyData = rd.readBodyData();
                    bodyData.copyTo(cap.body);
                    break;
            }

            return null; // something bad :(
        }
    }
}