using Sor.Game.Save;

namespace Sor.Game {
    public static class GameLoader {
        public static PlayPersistable loadSave(PlayState playState) {
            var store = NGame.context.data.getStore();
            var pers = new PlayPersistable(playState);
            store.Load(Constants.Game.GAME_SLOT_0, pers);
            return pers;
        }
    }
}