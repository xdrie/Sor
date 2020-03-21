using Microsoft.Xna.Framework.Input;
using Nez;

namespace Sor.Components.Input {
    public class PlayerInputController : InputController {
        public override void Initialize() {
            base.Initialize();

            var gamepadIndex = 0;
            
            moveTurnInput.AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.Left, Keys.Right);
            moveTurnInput.AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.J, Keys.L);
            moveTurnInput.AddGamePadLeftStickX(gamepadIndex);
            
            moveThrustInput.AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.Up, Keys.Down);
            moveThrustInput.AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.I, Keys.K);
            moveThrustInput.AddGamePadButtons(gamepadIndex, VirtualInput.OverlapBehavior.CancelOut,
                Buttons.RightTrigger, Buttons.LeftTrigger);
            
            // // invert gamepad sticks to match virtual keyboard input
            // Nez.Input.GamePads[gamepadIndex].IsLeftStickVerticalInverted = true;
            // Nez.Input.GamePads[gamepadIndex].IsRightStickVerticalInverted = true;

            interactInput.AddKeyboardKey(Keys.E);
            interactInput.AddGamePadButton(gamepadIndex, Buttons.Y);

            boostInput.AddKeyboardKey(Keys.LeftShift);
            boostInput.AddGamePadButton(0, Buttons.RightShoulder);

            fireInput.AddKeyboardKey(Keys.Space);
            fireInput.AddGamePadButton(0, Buttons.B);

            tetherInput.AddKeyboardKey(Keys.D2);
            tetherInput.AddGamePadButton(0, Buttons.X);
        }
    }
}