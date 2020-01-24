using System.Linq;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez.Persistence.Binary;
using Sor.Components.Units;
using Sor.Scenes;
using Sor.Util;

namespace Sor.Game {
    public class PlayPersistable : IPersistable {
        private PlayScene play;
        
        public bool loaded = false;
        public const int version = 2;
        
        // default values
        public Vector2 playerPosition = new Vector2(200, 200);

        public PlayPersistable(PlayScene play) {
            this.play = play;
        }

        public void Recover(IPersistableReader rd) {
            loaded = true;
            Global.log.writeLine($"{nameof(PlayPersistable)}::recover called", GlintLogger.LogLevel.Information);
            var readVersion = rd.ReadInt();
            if (version != readVersion) {
                Global.log.writeLine($"save file version mismatch (got {readVersion}, expected {version})", GlintLogger.LogLevel.Error);
            }

            // load all wings
            var wingCount = rd.ReadInt();
            for (var i = 0; i < wingCount; i++) {
                // TODO: figure out how the fuck to load a wing
            }
        }

        public void Persist(IPersistableWriter wr) {
            Global.log.writeLine($"{nameof(PlayPersistable)}::persist called", GlintLogger.LogLevel.Information);
            wr.Write(version); // save file version
            
            // save player
            wr.writeWing(play.playerWing);
            
            // save all other wings
            var wingsToSave = play.FindEntitiesWithTag(Constants.ENTITY_WING).ToList();
            wr.Write(wingsToSave.Count);
            foreach (var wingNt in wingsToSave) {
                var wing = wingNt.GetComponent<Wing>();
                wr.writeWing(wing);
            }
        }
    }
}