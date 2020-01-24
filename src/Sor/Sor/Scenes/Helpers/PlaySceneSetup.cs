using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI;
using Sor.AI.Cogs;
using Sor.Components.Input;
using Sor.Components.Units;
using Sor.Game;

namespace Sor.Scenes.Helpers {
    public class PlaySceneSetup {
        public GameContext gameContext;
        public PlayScene play;
        private PlayPersistable pers;

        public PlaySceneSetup(PlayScene play) {
            this.play = play;
            gameContext = Core.Services.GetService<GameContext>();
        }
        
        public PlayPersistable loadGame() {
            var store = gameContext.data.getStore();
            var pers = new PlayPersistable(this);
            store.Load(GameData.TEST_SAVE, pers);
            return pers;
        }
        
        public void createScene() {
            createPlayer(new Vector2(200, 200));

            var blockNt = play.CreateEntity("block", new Vector2(140, 140));
            var blockColl = blockNt.AddComponent(new BoxCollider(-4, -16, 8, 32));

            var mapAsset = Core.Content.LoadTiledMap("Data/maps/test2.tmx");
            var mapEntity = play.CreateEntity("map");
            var mapRenderer = mapEntity.AddComponent(new TiledMapRenderer(mapAsset, null, false));
            var loader = new MapLoader(play, mapEntity);
            loader.load(mapAsset);

            // load the game from data
            pers = loadGame();

            if (!pers.loaded) {
                // fresh
                createWing("duck-uno", new Vector2(-140, 320));
                createWing("duck-dos", new Vector2(-140, 20), BirdPersonality.makeNeutral());
            }

            var status = pers.loaded ? "recreated" : "freshly created";
            Global.log.writeLine($"play scene {status}", GlintLogger.LogLevel.Information);
        }

        public void createPlayer(Vector2 pos) {
            play.playerEntity = play.CreateEntity("player", pos).SetTag(Constants.ENTITY_WING);
            play.playerWing = play.playerEntity.AddComponent(new Wing());
            var playerSoul = new AvianSoul(BirdPersonality.makeNeutral());
            playerSoul.calc();
            var playerMind = play.playerWing.AddComponent(new Mind(playerSoul, false));
            play.playerEntity.AddComponent<PlayerInputController>();
        }

        public Wing createWing(string name, Vector2 pos, BirdPersonality ply = null) {
            var duckNt = play.CreateEntity(name, pos).SetTag(Constants.ENTITY_WING);
            duckNt.AddComponent<LogicInputController>();
            var duck = duckNt.AddComponent(new Wing());
            var duckSoul = new AvianSoul(ply);
            duckSoul.calc();
            var duckMind = duckNt.AddComponent(new Mind(duckSoul, false));
            return duck;
        }
    }
}