using Glint.Game;

namespace Sor.Game {
    public static class GameLoader {
        public static PlayPersistable loadGameInto(PlayContext playContext) {
            var store = NGame.context.data.getStore();
            var pers = new PlayPersistable(playContext);
            store.Load(GameData.TEST_SAVE, pers);
            return pers;
        }
    }
}