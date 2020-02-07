using Glint;
using Microsoft.Xna.Framework;

namespace Sor.Components.Input {
    public class LogicInputController : InputController {
        public VirtualJoystick.LogicJoystick moveDirectionLogical { get; } = new VirtualJoystick.LogicJoystick();
        public VirtualButton.LogicButton interactLogical { get; } = new VirtualButton.LogicButton();
        public VirtualButton.LogicButton tetherLogical { get; } = new VirtualButton.LogicButton();
        public VirtualButton.LogicButton boostLogical { get; } = new VirtualButton.LogicButton();
        
        public override void Initialize() {
            base.Initialize();
            
            moveDirectionInput.Nodes.Add(moveDirectionLogical);
            
            interactInput.Nodes.Add(interactLogical);
            tetherInput.Nodes.Add(tetherLogical);
            boostInput.Nodes.Add(boostLogical);
        }

        public void zero() {
            moveDirectionLogical.LogicValue = Vector2.Zero;
            interactLogical.logicPressed = false;
            tetherLogical.logicPressed = false;
            boostLogical.logicPressed = false;
        }
    }
}