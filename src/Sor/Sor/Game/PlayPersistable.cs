using System.Linq;
using Glint.Util;
using Microsoft.Xna.Framework;
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

        // default values
        public Vector2 playerPosition = new Vector2(200, 200);

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

            // read player
            var playerWd = rd.readWingMeta();
            play.playerWing.name = playerWd.name;
            play.playerWing.core.energy = playerWd.energy;
            play.playerWing.mind.soul.ply = playerWd.ply;
            var bodyData = rd.readBodyData();
            bodyData.copyTo(play.playerWing.body);

            // load all wings
            var wingCount = rd.ReadInt();
            for (var i = 0; i < wingCount; i++) {
                var wd = rd.readWingMeta();
                var wing = setup.createWing(wd.name, Vector2.Zero, new AvianSoul(wd.ply));
                var bd = rd.readBodyData();
                bd.copyTo(wing.body);
                wing.changeClass(wd.wingClass);
            }
        }

        public void Persist(IPersistableWriter wr) {
            Global.log.writeLine($"{nameof(PlayPersistable)}::persist called", GlintLogger.LogLevel.Information);
            wr.Write(version); // save file version

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
            foreach (var thingNt in thingsToSave) {
                var thing = thingNt.GetComponent<Thing>();
                var kind = ThingHelper.classify(thing);
                wr.saveThing(thing);
            }
        }
    }
}