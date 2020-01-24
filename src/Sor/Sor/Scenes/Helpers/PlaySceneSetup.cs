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
        private PlayScene play;

        public PlaySceneSetup(PlayScene play) {
            this.play = play;
        }
        
        public void createFreshGame() {
            play.playerEntity = play.CreateEntity("player", new Vector2(200, 200)).SetTag(Constants.ENTITY_WING);
            play.playerWing = play.playerEntity.AddComponent(new Wing());
            var playerSoul = new AvianSoul {ply = BirdPersonality.makeNeutral()};
            playerSoul.calculateTraits();
            var playerMind = play.playerWing.AddComponent(new Mind(playerSoul, false));
            playerSoul.mind = playerMind; // associate mind with soul
            play.playerEntity.AddComponent<PlayerInputController>();

            var duckUnoNt = play.CreateEntity("duck-uno", new Vector2(-140, 320)).SetTag(Constants.ENTITY_WING);
            var duckUno = duckUnoNt.AddComponent(new Predator());
            duckUno.AddComponent<LogicInputController>();
            duckUno.AddComponent<Mind>();
            duckUno.AddComponent(new MindDisplay(play.playerWing, true));

            var duckDosNt = play.CreateEntity("duck-dos", new Vector2(-140, 20)).SetTag(Constants.ENTITY_WING);
            var duckDos = duckDosNt.AddComponent(new Wing());
            var duckDosSoul = new AvianSoul {ply = BirdPersonality.makeNeutral()};
            duckDosSoul.calculateTraits();
            var duckDosMind = duckDosNt.AddComponent(new Mind(duckDosSoul, false));
            duckDosSoul.mind = duckDosMind; // associate mind with soul

            var blockNt = play.CreateEntity("block", new Vector2(140, 140));
            var blockColl = blockNt.AddComponent(new BoxCollider(-4, -16, 8, 32));

            var mapAsset = Core.Content.LoadTiledMap("Data/maps/test2.tmx");
            var mapEntity = play.CreateEntity("map");
            var mapRenderer = mapEntity.AddComponent(new TiledMapRenderer(mapAsset, null, false));
            var loader = new MapLoader(play, mapEntity);
            loader.load(mapAsset);

            Global.log.writeLine("fresh play scene created", GlintLogger.LogLevel.Information);
        }
    }
}