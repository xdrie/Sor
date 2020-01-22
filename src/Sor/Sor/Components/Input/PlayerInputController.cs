using Microsoft.Xna.Framework.Input;
using Nez;

namespace Sor.Components.Input {
    public class PlayerInputController : InputController {
        public override void Initialize() {
            base.Initialize();
            
            // moveDirectionInput.Nodes.Add(new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehavior.CancelOut,
            //     Keys.A, Keys.D, Keys.W, Keys.S));
            moveDirectionInput.Nodes.Add(new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehavior.CancelOut,
                Keys.Left, Keys.Right, Keys.Up, Keys.Down));
            moveDirectionInput.Nodes.Add(new VirtualJoystick.GamePadLeftStick());
            var gamepadIndex = 0;
            Nez.Input.GamePads[gamepadIndex].IsLeftStickVerticalInverted = true;
            Nez.Input.GamePads[gamepadIndex].IsRightStickVerticalInverted = true;
            
            interactInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.E));
            interactInput.Nodes.Add(new VirtualButton.GamePadButton(gamepadIndex, Buttons.Y));
            
            boostInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.LeftShift));
            
            tetherInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.D2));
        }
    }
}