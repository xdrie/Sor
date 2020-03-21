using Glint;
using Microsoft.Xna.Framework;

namespace Sor.Components.Input {
    public class LogicInputController : InputController {
        public VirtualAxis.LogicAxis moveTurnLogical;
        public VirtualAxis.LogicAxis moveThrustLogical;
        public VirtualButton.LogicButton interactLogical;
        public VirtualButton.LogicButton tetherLogical;
        public VirtualButton.LogicButton boostLogical;
        public VirtualButton.LogicButton fireLogical;
        
        public override void Initialize() {
            base.Initialize();
            
            moveTurnLogical = moveTurnInput.AddLogical();
            moveThrustLogical = moveThrustInput.AddLogical();

            interactLogical = interactInput.AddLogical();
            tetherLogical = tetherInput.AddLogical();
            boostLogical = boostInput.AddLogical();
            fireLogical = fireInput.AddLogical();
        }

        public void zero() {
            moveTurnLogical.LogicValue = 0;
            moveThrustLogical.LogicValue = 0;
            interactLogical.LogicPressed = false;
            tetherLogical.LogicPressed = false;
            boostLogical.LogicPressed = false;
            fireLogical.LogicPressed = false;
        }
    }
}