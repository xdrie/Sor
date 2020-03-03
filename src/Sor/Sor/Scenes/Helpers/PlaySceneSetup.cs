using Glint;
using Glint.Game;
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
            // TODO: ensure that the loaded map matches the saved map
            var mapEntity = play.CreateEntity("map");
            var mapRenderer = mapEntity.AddComponent(new TiledMapRenderer(mapAsset, null, false));
            var mapLoader = new MapLoader(play, mapEntity);

            // load the game from data
            pers = loadGame();

            if (!pers.loaded) {
                // fresh
                var uno = play.createWing("uno", new Vector2(-140, 320));
                uno.changeClass(Wing.WingClass.Predator);
                var frend = play.createWing("frend", new Vector2(-140, 20),
                    new AvianSoul(new BirdPersonality {A = -0.8f, S = 0.7f}));
                
                // var tres = createWing("tres", new Vector2(200, 380));
                // var cuatro = createWing("cuatro", new Vector2(0, -100));
                // var cinco = createWing("cinco", new Vector2(400, 100));
                
                mapLoader.load(mapAsset, createObjects: true);
            } else {
                // resuming from saved state
                mapLoader.load(mapAsset, createObjects: false); // entities are already repopulated
            }
            
            gameContext.map = mapLoader.mapRepr; // copy map representation

            var status = pers.loaded ? "recreated" : "freshly created";
            Global.log.writeLine($"play scene {status}", GlintLogger.LogLevel.Information);
        }

        public void createPlayer(Vector2 pos) {
            play.playerEntity = play.CreateEntity("player", pos).SetTag(Constants.ENTITY_WING);
            var playerSoul = new AvianSoul(BirdPersonality.makeNeutral());
            playerSoul.calc();
            play.playerWing = play.playerEntity.AddComponent(new Wing(new Mind(playerSoul, false)));
            play.playerEntity.AddComponent<PlayerInputController>();
        }
    }
}