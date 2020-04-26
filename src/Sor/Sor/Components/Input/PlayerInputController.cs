using Microsoft.Xna.Framework.Input;
using Nez;

namespace Sor.Components.Input {
    public class PlayerInputController : InputController {
        public int index { get; }

        public PlayerInputController(int index) {
            this.index = index;
        }

        public override void Initialize() {
            base.Initialize();

            moveTurnInput.AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.Left, Keys.Right);
            moveTurnInput.AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.J, Keys.L);
            moveTurnInput.AddGamePadLeftStickX(index);
            moveTurnInput.AddGamePadDPadLeftRight(index);

            moveThrustInput.AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.Up, Keys.Down);
            moveThrustInput.AddKeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.I, Keys.K);
            moveThrustInput.AddGamePadButtons(index, VirtualInput.OverlapBehavior.CancelOut,
                Buttons.A, Buttons.LeftTrigger);
            moveThrustInput.AddGamePadDPadUpDown(index);

            // // invert gamepad sticks to match virtual keyboard input
            // Nez.Input.GamePads[gamepadIndex].IsLeftStickVerticalInverted = true;
            // Nez.Input.GamePads[gamepadIndex].IsRightStickVerticalInverted = true;

            interactInput.AddKeyboardKey(Keys.E);
            interactInput.AddGamePadButton(index, Buttons.Y);

            boostInput.AddKeyboardKey(Keys.LeftShift);
            boostInput.AddGamePadButton(index, Buttons.RightTrigger);

            fireInput.AddKeyboardKey(Keys.Space);
            fireInput.AddGamePadButton(index, Buttons.B);

            tetherInput.AddKeyboardKey(Keys.D2);
            tetherInput.AddGamePadButton(index, Buttons.X);
        }
    }
}