using Glint.Util;
using Microsoft.Xna.Framework;
using Nez.Persistence.Binary;
using Sor.Scenes;
using Sor.Util;

namespace Sor.Game {
    public class PlayStatePersistable : IPersistable {
        private PlayScene play;
        
        public bool loaded = false;
        public const int version = 2;
        
        // default values
        public Vector2 playerPosition = new Vector2(200, 200);

        public PlayStatePersistable(PlayScene play) {
            this.play = play;
        }

        public void Recover(IPersistableReader reader) {
            loaded = true;
            Global.log.writeLine($"{nameof(PlayStatePersistable)}::recover called", GlintLogger.LogLevel.Information);
            var readVersion = reader.ReadInt();
            if (version != readVersion) {
                Global.log.writeLine($"save file version mismatch (got {readVersion}, expected {version})", GlintLogger.LogLevel.Error);
            }

            playerPosition = reader.ReadVec2();
        }

        public void Persist(IPersistableWriter writer) {
            Global.log.writeLine($"{nameof(PlayStatePersistable)}::persist called", GlintLogger.LogLevel.Information);
            writer.Write(version); // save file version
            
            // save player
            writer.Write(play.playerEntity.Position);
        }
    }
}