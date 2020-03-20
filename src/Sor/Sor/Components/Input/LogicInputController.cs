using Glint;
using Microsoft.Xna.Framework;

namespace Sor.Components.Input {
    public class LogicInputController : InputController {
        public VirtualJoystick.LogicJoystick moveDirectionLogical;
        public VirtualButton.LogicButton interactLogical;
        public VirtualButton.LogicButton tetherLogical;
        public VirtualButton.LogicButton boostLogical;
        public VirtualButton.LogicButton fireLogical;
        
        public override void Initialize() {
            base.Initialize();
            
            moveDirectionLogical = moveDirectionInput.AddLogical();

            interactLogical = interactInput.AddLogical();
            tetherLogical = tetherInput.AddLogical();
            boostLogical = boostInput.AddLogical();
            fireLogical = fireInput.AddLogical();
        }

        public void zero() {
            moveDirectionLogical.LogicValue = Vector2.Zero;
            interactLogical.LogicPressed = false;
            tetherLogical.LogicPressed = false;
            boostLogical.LogicPressed = false;
        }
    }
}