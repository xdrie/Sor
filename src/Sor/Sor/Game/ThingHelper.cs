using Glint.Util;
using Nez.Persistence.Binary;
using Sor.Components.Things;
using Sor.Util;

namespace Sor.Game {
    public class ThingHelper {
        private PlayPersistable pers;

        public enum ThingKind {
            Unknown,
            Capsule,
            Tree
        }

        public ThingHelper(PlayPersistable pers) {
            this.pers = pers;
        }

        public ThingKind classify(Thing thing) {
            if (thing is Capsule) return ThingKind.Capsule;
            if (thing is Tree) return ThingKind.Tree;
            return ThingKind.Unknown;
        }

        public void saveThing(IPersistableWriter wr, Thing thing) {
            wr.Write((int) classify(thing));
            switch (thing) {
                case Capsule cap: {
                    // write status
                    wr.Write(cap.acquired);
                    // write body data
                    wr.writeBody(cap.body);
                    // write other capsule info
                    wr.Write(cap.energy);
                    wr.Write(cap.firstAvailableAt);
                    wr.Write(cap.despawnAt);
                    wr.Write(cap.sender?.name);
                    wr.Write(cap.creator?.bark);
                    break;
                }
                case Tree tree: {
                    // TODO: too lazy to store fruit refs
                    wr.Write(tree.stage);
                    wr.Write(tree.harvest);
                    wr.Write(tree.bark);

                    break;
                }
            }
        }

        public Thing loadThing(IPersistableReader rd) {
            var kind = (ThingKind) rd.ReadInt();
            switch (kind) {
                case ThingKind.Unknown:
                    // unrecognized thing
                    Global.log.writeLine("unrecognized thing kind", GlintLogger.LogLevel.Error);
                    return null;
                case ThingKind.Capsule: {
                    // don't load acquired capsules
                    var acquired = rd.ReadBool();
                    if (acquired) return null;
                    var nt = pers.play.CreateEntity("cap");
                    var cap = nt.AddComponent(new Capsule());
                    // read body
                    var bodyData = rd.readBodyData();
                    bodyData.copyTo(cap.body);
                    // read other capsule info
                    cap.energy = rd.ReadFloat();
                    cap.firstAvailableAt = rd.ReadFloat();
                    cap.despawnAt = rd.ReadFloat();
                    var senderName = rd.ReadString();
                    if (senderName != null) {
                        cap.sender = pers.wings.Find(x => x.name == senderName);
                    }

                    var treeBark = rd.ReadString();
                    if (treeBark != null) {
                        cap.creator = pers.trees.Find(x => x.bark == treeBark);
                    }

                    return cap;
                }

                case ThingKind.Tree: {
                    var nt = pers.play.CreateEntity("tree");
                    var tree = nt.AddComponent(new Tree());
                    // load tree
                    tree.stage = rd.ReadInt();
                    tree.harvest = rd.ReadInt();
                    tree.bark = rd.ReadString();

                    tree.updateStage();
                    pers.trees.Add(tree); // add tree to working list
                    return tree;
                }
            }

            return null; // something bad :(
        }
    }
}