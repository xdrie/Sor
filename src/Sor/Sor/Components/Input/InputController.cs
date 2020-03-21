using Nez;

namespace Sor.Components.Input {
    public abstract class InputController : Component {
        public VirtualButton interactInput = new VirtualButton();
        
        public VirtualAxis moveTurnInput = new VirtualAxis();
        public VirtualAxis moveThrustInput = new VirtualAxis();
        
        public VirtualButton boostInput = new VirtualButton();
        public VirtualButton fireInput = new VirtualButton();
        
        public VirtualButton tetherInput = new VirtualButton();
    }
}