namespace Sor.Game {
    public static class GameLoader {
        public static PlayPersistable loadSave(PlayContext playContext) {
            var store = NGame.context.data.getStore();
            var pers = new PlayPersistable(playContext);
            store.Load(Constants.Game.GAME_SLOT_0, pers);
            return pers;
        }
    }
}