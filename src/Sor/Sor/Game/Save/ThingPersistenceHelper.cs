using Glint;
using Glint.Util;
using Nez;
using Nez.Persistence.Binary;
using Sor.Components.Things;
using Sor.Util;

namespace Sor.Game.Save {
    public class ThingPersistenceHelper {
        private PlayPersistable per;

        public enum ThingKind {
            Unknown,
            Capsule,
            Tree
        }

        public ThingPersistenceHelper(PlayPersistable per) {
            this.per = per;
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
                    wr.Write(cap.sender?.name ?? string.Empty);
                    wr.Write(cap.creator?.bark ?? string.Empty);
                    break;
                }
                case Tree tree: {
                    // TODO: too lazy to store fruit refs
                    wr.Write(tree.Transform.Position);
                    wr.Write(tree.stage);
                    wr.Write(tree.harvest);
                    wr.Write(tree.bark);

                    break;
                }
            }
        }

        public Thing loadThing(IPersistableReader rd) {
            var kind = (ThingKind) rd.ReadInt();
            var res = default(Thing);
            switch (kind) {
                case ThingKind.Capsule: {
                    var nt = new Entity("cap");
                    var cap = nt.AddComponent(new Capsule());
                    cap.acquired = rd.ReadBool();
                    // read body
                    var bodyData = rd.readBody();
                    bodyData.copyTo(cap.body);
                    // read other capsule info
                    cap.energy = rd.ReadFloat();
                    cap.firstAvailableAt = rd.ReadFloat();
                    cap.despawnAt = rd.ReadFloat();
                    var senderName = rd.ReadString();
                    if (!string.IsNullOrWhiteSpace(senderName)) {
                        cap.sender = per.wings.Find(x => x.name == senderName);
                    }

                    var treeBark = rd.ReadString();
                    if (!string.IsNullOrWhiteSpace(treeBark)) {
                        cap.creator = per.trees.Find(x => x.bark == treeBark);
                    }

                    // if acquired then throw away
                    if (cap.acquired) {
                        cap = null; // ick
                    }

                    res = cap;
                    break;
                }

                case ThingKind.Tree: {
                    var nt = new Entity("tree");
                    var tree = nt.AddComponent(new Tree());
                    // load tree
                    tree.Entity.Position = rd.ReadVec2();
                    tree.stage = rd.ReadInt();
                    tree.harvest = rd.ReadInt();
                    tree.bark = rd.ReadString();

                    tree.updateStage();
                    per.trees.Add(tree); // add tree to working list
                    res = tree;
                    break;
                }
                default:
                    // unrecognized thing
                    Global.log.err("unrecognized thing kind");
                    res = null;
                    break;
            }

            if (res != null) {
                Global.log.trace($"rehydrated entity {res.GetType().Name}, pos{res.Entity.Position.RoundToPoint()}");
            }

            return res; // yee
        }
    }
}