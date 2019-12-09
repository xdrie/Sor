using Microsoft.Xna.Framework;
using Sor.Components.Camera;
using Sor.Components.Units;

namespace Sor.Scenes {
    public class PlayScene : BaseGameScene {
        public override void Initialize() {
            base.Initialize();
            
            ClearColor = gameContext.assets.bgColor;

            var playerEntity = CreateEntity("player", new Vector2(200, 200));
            var playerShip = playerEntity.AddComponent(new Ship());
            
            // add component to make Camera follow the player
            // var followCamera = 
            //     Camera.Entity.AddComponent(new LockedCamera(playerEntity, Camera, LockedCamera.LockMode.Position));
        }
    }
}