using Microsoft.Xna.Framework.Input;
using Nez;

namespace Sor.Components.Input {
    public class PlayerInputController : InputController {
        public override void Initialize() {
            base.Initialize();

            // moveDirectionInput.Nodes.Add(new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehavior.CancelOut,
            //     Keys.A, Keys.D, Keys.W, Keys.S));
            moveDirectionInput
                .AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut,
                    Keys.Left, Keys.Right, Keys.Up, Keys.Down);
            moveDirectionInput
                .AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut,
                    Keys.J, Keys.L, Keys.I, Keys.K);
            var gamepadIndex = 0;
            moveDirectionInput.AddGamePadLeftStick(gamepadIndex);
            // invert gamepad sticks to match virtual keyboard input
            Nez.Input.GamePads[gamepadIndex].IsLeftStickVerticalInverted = true;
            Nez.Input.GamePads[gamepadIndex].IsRightStickVerticalInverted = true;

            interactInput.AddKeyboardKey(Keys.E);
            interactInput.AddGamePadButton(gamepadIndex, Buttons.Y);

            boostInput.AddKeyboardKey(Keys.LeftShift);

            fireInput.AddKeyboardKey(Keys.Space);

            tetherInput.AddKeyboardKey(Keys.D2);
        }
    }
}