using Glint.Game;

namespace Sor.Game {
    public static class GameLoader {
        public static PlayPersistable loadSave(PlayContext playContext) {
            var store = NGame.context.data.getStore();
            var pers = new PlayPersistable(playContext);
            store.Load(GameData<Config>.TEST_SAVE, pers);
            return pers;
        }
    }
}