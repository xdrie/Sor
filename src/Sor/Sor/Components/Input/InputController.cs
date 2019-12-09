using Nez;

namespace Sor.Components.Input {
    public abstract class InputController : Component {
        public VirtualButton interactInput = new VirtualButton();
        public VirtualJoystick moveDirectionInput = new VirtualJoystick(true);
    }
}