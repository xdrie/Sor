using Glint.Util;
using Nez.Persistence.Binary;
using Sor.Scenes;

namespace Sor.Game {
    public class PlayStatePersistable : IPersistable {
        private PlayScene play;

        private int test;

        public PlayStatePersistable(PlayScene play) {
            this.play = play;
        }

        public void Recover(IPersistableReader reader) {
            Global.log.writeLine($"{nameof(PlayStatePersistable)}::recover called", GlintLogger.LogLevel.Information);
            test = reader.ReadInt();
        }

        public void Persist(IPersistableWriter writer) {
            Global.log.writeLine($"{nameof(PlayStatePersistable)}::persist called", GlintLogger.LogLevel.Information);
            writer.Write(1); // test
        }
    }
}