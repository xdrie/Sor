using System.Collections.Generic;
using System.Linq;
using Glint;
using Glint.Util;
using Nez;
using Nez.Persistence.Binary;
using Sor.Components.Things;
using Sor.Util;

namespace Sor.Game.Save {
    public class ThingLoader {
        private PlayPersistable per;

        public enum ThingKind {
            Unknown,
            Capsule,
            Tree
        }

        public class LoadedThing : Loaded<Thing> {
            public LoadedThing(Thing instance) : base(instance) { }

            /// <summary>
            /// the one that last interacted with the thing
            /// </summary>
            public long interactorUid;

            /// <summary>
            /// the one that created the thing
            /// </summary>
            public long creatorUid;
        }

        public ThingLoader(PlayPersistable per) {
            this.per = per;
        }

        /// <summary>
        /// get a numerical type identifier for the thing
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        public ThingKind classify(Thing thing) {
            if (thing is Capsule) return ThingKind.Capsule;
            if (thing is Tree) return ThingKind.Tree;
            return ThingKind.Unknown;
        }

        public void saveThing(IPersistableWriter wr, Thing thing) {
            var kindId = (int) classify(thing);
            // thing header
            wr.Write(kindId);
            wr.Write(thing.uid);
            // thing data
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
                    wr.Write(cap.interactor?.uid ?? default);
                    wr.Write(cap.creator?.uid ?? default);
                    break;
                }
                case Tree tree: {
                    // we don't store fruit refs
                    wr.Write(tree.Transform.Position);
                    wr.Write(tree.stage);
                    wr.Write(tree.harvest);

                    break;
                }
            }
        }

        public LoadedThing loadThing(IPersistableReader rd) {
            var kind = (ThingKind) rd.ReadInt();
            var res = default(Thing);
            var uid = rd.ReadLong();
            var senderUid = default(long);
            var creatorUid = default(long);
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
                    senderUid = rd.ReadLong();
                    creatorUid = rd.ReadLong();

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
                    tree.uid = uid;

                    tree.updateStage();
                    res = tree;
                    break;
                }
                default:
                    // unrecognized thing
                    Global.log.err($"unrecognized thing kind: {kind}");
                    res = null;
                    break;
            }

            if (res != null) {
                Global.log.trace($"rehydrated entity {res.GetType().Name}, pos{res.Entity.Position.RoundToPoint()}");

                var loadedThing = new LoadedThing(res) {
                    interactorUid = senderUid,
                    creatorUid = creatorUid
                };
                return loadedThing; // yee
            }

            return null;
        }

        public List<Thing> resolveThings(IEnumerable<LoadedThing> loads) {
            var things = new List<Thing>();
            foreach (var load in loads) {
                // process the load...
                switch (load.instance) {
                    case Capsule cap:
                        // load wing ref
                        if (load.interactorUid > 0) {
                            cap.interactor = per.setup.wings.Single(x => x.uid == load.interactorUid);
                        }

                        // load tree ref
                        if (load.creatorUid > 0) {
                            cap.creator = (Tree) loads.Select(x => x.instance)
                                .Single(x => (x as Tree)?.uid == load.creatorUid);
                        }

                        break;
                }

                GAssert.Ensure(load.instance != null, "the instance of a load was null");
                // now add it to the finished list
                things.Add(load.instance);
            }

            return things;
        }
    }
}