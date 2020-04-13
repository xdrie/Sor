using System.Collections.Generic;
using System.Linq;
using Glint;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence.Binary;
using Sor.Components.Items;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Util;

namespace Sor.Game {
    public class PlayPersistable : IPersistable {
        public PlayContext playContext;

        public bool loaded = false;
        public const int version = 4;
        public const float timeAdvance = 30f; // time to advance when loading

        // helper values
        public List<Wing> wings = new List<Wing>();
        public List<Tree> trees = new List<Tree>();

        public PlayPersistable(PlayContext playContext) {
            this.playContext = playContext;
        }

        public void Recover(IPersistableReader rd) {
            loaded = true;
            Global.log.writeLine($"{nameof(PlayPersistable)}::recover called", Logger.Verbosity.Information);
            var readVersion = rd.ReadInt();
            if (version != readVersion) {
                Global.log.writeLine($"save file version mismatch (got {readVersion}, expected {version})",
                    Logger.Verbosity.Error);
            }

            // load game time
            Time.TotalTime = rd.ReadFloat() + timeAdvance;
            
            // load map seed
            playContext.mapgenSeed = rd.ReadInt();
            Global.log.writeLine($"loaded mapgen seed: {playContext.mapgenSeed}", Logger.Verbosity.Trace);
            
            // set rehydrated flag
            playContext.rehydrated = true;

            // read player
            var playerWd = rd.readWingMeta();
            var playerBodyData = rd.readBodyData();
            var player = playContext.createPlayer(playerBodyData.pos);
            player.name = playerWd.name;
            player.core.energy = playerWd.energy;
            player.mind.soul.ply = playerWd.ply;
            playerBodyData.copyTo(playContext.playerWing.body);
            player.changeClass(playerWd.wingClass);
            if (playerWd.armed) {
                player.AddComponent<Shooter>();
            }
            wings.Add(playContext.playerWing);

            // load all wings
            var wingCount = rd.ReadInt();
            for (var i = 0; i < wingCount; i++) {
                var wd = rd.readWingMeta();
                var wing = playContext.createWing(wd.name, Vector2.Zero, wd.ply);
                if (wd.armed) {
                    wing.AddComponent<Shooter>();
                }
                var bd = rd.readBodyData();
                bd.copyTo(wing.body);
                wing.changeClass(wd.wingClass);
                wings.Add(wing);
                Global.log.writeLine($"rehydrated wing {wing.name}, pos{wing.body.pos.RoundToPoint()}, ply{wing.mind.soul.ply}", Logger.Verbosity.Trace);
            }

            // load world things
            var thingCount = rd.ReadInt();
            for (var i = 0; i < thingCount; i++) {
                var thingHelper = new ThingHelper(this);
                // load and inflate thing
                var thing = thingHelper.loadThing(rd);
                if (thing != null) { // thing might not be loadedF
                    // tag entity as thing
                    thing.Entity.SetTag(Constants.Tags.THING);
                    // add to context
                    playContext.addThing(thing);
                }
            }
        }

        public void Persist(IPersistableWriter wr) {
            Global.log.writeLine($"{nameof(PlayPersistable)}::persist called", Logger.Verbosity.Information);
            wr.Write(version); // save file version

            // save game time
            wr.Write(Time.TotalTime);
            
            // save map seed
            wr.Write(playContext.mapgenSeed);

            // save player
            wr.writeWingMeta(playContext.playerWing);
            wr.writeBody(playContext.playerWing.body);

            // save all other wings
            var wingsToSave = playContext.scene.FindEntitiesWithTag(Constants.Tags.WING)
                .Where(x => x != playContext.playerWing.Entity)
                .ToList();
            wr.Write(wingsToSave.Count);
            foreach (var wingNt in wingsToSave) {
                var wing = wingNt.GetComponent<Wing>();
                wr.writeWingMeta(wing);
                wr.writeBody(wing.body);
            }

            // save world things
            var thingsToSave = playContext.scene.FindEntitiesWithTag(Constants.Tags.THING).ToList();
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
            var thingHelper = new ThingHelper(this);
            foreach (var thing in saveThingList) {
                var kind = thingHelper.classify(thing);
                thingHelper.saveThing(wr, thing);
            }
        }
    }
}