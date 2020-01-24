using System.Collections.Generic;
using System.Linq;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Persistence.Binary;
using Sor.AI.Cogs;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Scenes;
using Sor.Scenes.Helpers;
using Sor.Util;

namespace Sor.Game {
    public class PlayPersistable : IPersistable {
        public PlayScene play;
        public PlaySceneSetup setup;

        public bool loaded = false;
        public const int version = 3;

        // helper values
        public List<Wing> wings = new List<Wing>();
        public List<Tree> trees = new List<Tree>();

        public PlayPersistable(PlaySceneSetup setup) {
            this.play = setup.play;
            this.setup = setup;
        }

        public void Recover(IPersistableReader rd) {
            loaded = true;
            Global.log.writeLine($"{nameof(PlayPersistable)}::recover called", GlintLogger.LogLevel.Information);
            var readVersion = rd.ReadInt();
            if (version != readVersion) {
                Global.log.writeLine($"save file version mismatch (got {readVersion}, expected {version})",
                    GlintLogger.LogLevel.Error);
            }
            
            // load game time
            Time.TotalTime = rd.ReadFloat();

            // read player
            var playerWd = rd.readWingMeta();
            play.playerWing.name = playerWd.name;
            play.playerWing.core.energy = playerWd.energy;
            play.playerWing.mind.soul.ply = playerWd.ply;
            var bodyData = rd.readBodyData();
            bodyData.copyTo(play.playerWing.body);
            wings.Add(play.playerWing);

            // load all wings
            var wingCount = rd.ReadInt();
            for (var i = 0; i < wingCount; i++) {
                var wd = rd.readWingMeta();
                var wing = setup.createWing(wd.name, Vector2.Zero, new AvianSoul(wd.ply));
                var bd = rd.readBodyData();
                bd.copyTo(wing.body);
                wing.changeClass(wd.wingClass);
                wings.Add(wing);
            }
            
            // load world things
            var thingCount = rd.ReadInt();
            for (var i = 0; i < thingCount; i++) {
                var thingHelper = new ThingHelper(this);
                // load and inflate thing
                var thing = thingHelper.loadThing(rd);
                // tag entity as thing
                thing.Entity.SetTag(Constants.ENTITY_THING);
            }
        }

        public void Persist(IPersistableWriter wr) {
            Global.log.writeLine($"{nameof(PlayPersistable)}::persist called", GlintLogger.LogLevel.Information);
            wr.Write(version); // save file version
            
            // save game time
            wr.Write(Time.TotalTime);

            // save player
            wr.writeWingMeta(play.playerWing);
            wr.writeBody(play.playerWing.body);

            // save all other wings
            var wingsToSave = play.FindEntitiesWithTag(Constants.ENTITY_WING)
                .Where(x => x != play.playerEntity)
                .ToList();
            wr.Write(wingsToSave.Count);
            foreach (var wingNt in wingsToSave) {
                var wing = wingNt.GetComponent<Wing>();
                wr.writeWingMeta(wing);
                wr.writeBody(wing.body);
            }
            
            // save world things
            var thingsToSave = play.FindEntitiesWithTag(Constants.ENTITY_THING).ToList();
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