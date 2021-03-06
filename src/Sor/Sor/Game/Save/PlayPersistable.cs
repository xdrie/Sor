using System.Collections.Generic;
using System.Linq;
using Glint;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence.Binary;
using Sor.Components.Items;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Util;

namespace Sor.Game.Save {
    public class PlayPersistable : IPersistable {
        public PlaySetup setup;

        public const int version = 5;
        public const float SAVE_TIME_ADVANCE = 30f; // time to advance when loading

        public PlayPersistable(PlaySetup setup) {
            this.setup = setup;
        }

        public void Recover(IPersistableReader rd) {
            Global.log.info($"{nameof(PlayPersistable)}::recover called");

            // verify save file version
            var readVersion = rd.ReadInt();
            if (version != readVersion) {
                Global.log.err($"save file version mismatch (got {readVersion}, expected {version})");
            }

            setup.rehydrated = true; // indicate that this play context is rehydrated
            // load game time
            Time.TotalTime = rd.ReadFloat() + SAVE_TIME_ADVANCE;

            // load map seed
            setup.state.mapgenSeed = rd.ReadInt();
            Global.log.trace($"loaded mapgen seed: {setup.state.mapgenSeed}");

            // read player
            var playerWd = rd.readWing();
            var playerBodyData = rd.readBody();
            var player = setup.createPlayer(Vector2.Zero);
            player.uid = playerWd.uid;
            player.core.energy = playerWd.energy;
            player.mind.soul.ply = playerWd.ply;
            playerBodyData.copyTo(player.body);
            player.changeClass(playerWd.wingClass);
            if (playerWd.armed) {
                player.AddComponent<Shooter>();
            }

            // load all wings
            var wingCount = rd.ReadInt();
            for (var i = 0; i < wingCount; i++) {
                var wd = rd.readWing();
                var wing = setup.createNpcWing(wd.name, Vector2.Zero, wd.ply);
                wing.uid = wd.uid;
                if (wd.armed) {
                    wing.AddComponent<Shooter>();
                }

                var bd = rd.readBody();
                // rd.readWingMemory(wing.mind);
                bd.copyTo(wing.body);
                wing.changeClass(wd.wingClass);
                Global.log.trace(
                    $"rehydrated wing {wing.name}, pos{wing.body.pos.RoundToPoint()}, ply{wing.mind.soul.ply}");
            }

            // load world things
            var thingCount = rd.ReadInt();
            var thingHelper = new ThingLoader(this);
            var loadedThings = new List<ThingLoader.LoadedThing>();
            for (var i = 0; i < thingCount; i++) {
                // load and inflate thing
                var load = thingHelper.loadThing(rd);
                if (load != null) {
                    loadedThings.Add(load);
                }
                else {
                    Global.log.err("attempted to load thing, but it was NULL");
                }
            }

            // now resolve all things
            var resolvedThings = thingHelper.resolveThings(loadedThings);
            foreach (var thing in resolvedThings) {
                // add to context
                setup.addThing(thing);
            }
        }

        public void Persist(IPersistableWriter wr) {
            Global.log.info($"{nameof(PlayPersistable)}::persist called");
            wr.Write(version); // save file version

            // save game time
            wr.Write(Time.TotalTime);

            // save map seed
            wr.Write(setup.state.mapgenSeed);

            // save player
            wr.writeWing(setup.state.player);
            wr.writeBody(setup.state.player.body);

            // save all other wings
            var wingsToSave = setup.state.scene.FindEntitiesWithTag(Constants.Tags.WING)
                .Where(x => x != setup.state.player.Entity)
                .ToList();
            wr.Write(wingsToSave.Count);
            foreach (var wingNt in wingsToSave) {
                var wing = wingNt.GetComponent<Wing>();
                wr.writeWing(wing);
                wr.writeBody(wing.body);
                // wr.writeWingMemory(wing.mind);
            }

            // save world things
            var thingsToSave = setup.state.scene.FindEntitiesWithTag(Constants.Tags.THING).ToList();
            wr.Write(thingsToSave.Count);
            // sort so trees are before capsules
            var treeList = new List<Thing>();
            var capList = new List<Thing>();
            foreach (var thingNt in thingsToSave) {
                if (thingNt.HasComponent<Tree>()) {
                    treeList.Add(thingNt.GetComponent<Tree>());
                }

                if (thingNt.HasComponent<Capsule>()) {
                    capList.Add(thingNt.GetComponent<Capsule>());
                }
            }

            var saveThingList = new List<Thing>();
            saveThingList.AddRange(treeList);
            saveThingList.AddRange(capList);
            var thingHelper = new ThingLoader(this);
            foreach (var thing in saveThingList) {
                // var kind = thingHelper.classify(thing);
                thingHelper.saveThing(wr, thing);
            }
        }
    }
}