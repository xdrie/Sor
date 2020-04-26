using Sor.Game.Save;

namespace Sor.Game {
    public static class GameLoader {
        public static PlayPersistable loadSave(PlayState state) {
            var store = NGame.context.data.getStore();
            var pers = new PlayPersistable(state);
            store.Load(Constants.Game.GAME_SLOT_0, pers);
            return pers;
        }
    }
}