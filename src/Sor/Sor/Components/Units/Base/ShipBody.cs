using Glint.Physics;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Input;

namespace Sor.Components.Units {
    public class ShipBody : KinematicBody {
        public Ship me;
        private InputController controller;

        public float turnPower = Mathf.PI / 8f;
        public float thrustPower = 10f;

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            
            me = Entity.GetComponent<Ship>();
            controller = Entity.GetComponent<InputController>();

            maxAngular = turnPower * 2f;
            angularDrag = turnPower / 4f;
            drag = new Vector2(thrustPower / 4f);
            maxVelocity = new Vector2(thrustPower * 20f);
        }

        public override void Update() {
            base.Update();

            if (controller != null) {
                movement();
            }
        }

        private void movement() {
            var turnInput = controller.moveDirectionInput.Value.X;
            angularVelocity += turnInput * turnPower;
            var thrustInput = controller.moveDirectionInput.Value.Y;
            var thrustVec = new Vector2(0, thrustInput * thrustPower);
            velocity += thrustVec.rotate(angle);
        }
    }
}