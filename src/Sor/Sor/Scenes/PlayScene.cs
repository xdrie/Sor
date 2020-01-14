using Microsoft.Xna.Framework;
using Nez;
using Sor.AI;
using Sor.Components.Camera;
using Sor.Components.Input;
using Sor.Components.Units;
using Sor.Game;

namespace Sor.Scenes {
    public class PlayScene : BaseGameScene {
        public override void Initialize() {
            base.Initialize();
            
            ClearColor = gameContext.assets.bgColor;

            var playerEntity = CreateEntity("player", new Vector2(200, 200));
            var playerShip = playerEntity.AddComponent(new Wing());
            playerEntity.AddComponent<PlayerInputController>();
            
            var testEntity = CreateEntity("test1", new Vector2(320, 320));
            var testShip = testEntity.AddComponent(new Wing());
            testShip.AddComponent<LogicInputController>();
            testShip.AddComponent<Mind>();

            var blockNt = CreateEntity("block", new Vector2(140, 140));
            var blockColl = blockNt.AddComponent(new BoxCollider(-4, -16, 8, 32));

            var mapAsset = Core.Content.LoadTiledMap("Raw/maps/test1.tmx");
            var mapEntity = CreateEntity("map");
            var mapRenderer = mapEntity.AddComponent(new TiledMapRenderer(mapAsset, null, false));
            var loader = new MapLoader(this, mapEntity);
            loader.load(mapAsset);

            // Core.DebugRenderEnabled = true;

            // add component to make Camera follow the player
            var followCamera = 
                Camera.Entity.AddComponent(new LockedCamera(playerEntity, Camera, LockedCamera.LockMode.Position));
        }
    }
}