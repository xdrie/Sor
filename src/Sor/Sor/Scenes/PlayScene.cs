using Microsoft.Xna.Framework;
using Sor.Components.Camera;
using Sor.Components.Input;
using Sor.Components.Units;

namespace Sor.Scenes {
    public class PlayScene : BaseGameScene {
        public override void Initialize() {
            base.Initialize();
            
            ClearColor = gameContext.assets.bgColor;

            var playerEntity = CreateEntity("player", new Vector2(200, 200));
            var playerShip = playerEntity.AddComponent(new Ship());
            playerEntity.AddComponent<PlayerInputController>();
            
            var testEntity = CreateEntity("test1", new Vector2(320, 320));
            var testShip = testEntity.AddComponent(new Ship());

            // add component to make Camera follow the player
            var followCamera = 
                Camera.Entity.AddComponent(new LockedCamera(playerEntity, Camera, LockedCamera.LockMode.Position));
        }
    }
}