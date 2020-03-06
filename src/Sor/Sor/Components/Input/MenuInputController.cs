using Microsoft.Xna.Framework.Input;
using Nez;

namespace Sor.Components.Input {
    public class MenuInputController : Component {
        public VirtualButton navDown = new VirtualButton();
        public VirtualButton navUp = new VirtualButton();
        public VirtualButton interact = new VirtualButton();

        public override void Initialize() {
            base.Initialize();
            
            navDown.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Down));
            navDown.Nodes.Add(new VirtualButton.KeyboardKey(Keys.J));
            navDown.Nodes.Add(new VirtualButton.KeyboardKey(Keys.S));
            navDown.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.DPadUp));
            
            navUp.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Up));
            navUp.Nodes.Add(new VirtualButton.KeyboardKey(Keys.K));
            navUp.Nodes.Add(new VirtualButton.KeyboardKey(Keys.W));
            navUp.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.DPadDown));
            
            interact.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Enter));
            interact.Nodes.Add(new VirtualButton.KeyboardKey(Keys.E));
            interact.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.A));
        }
    }
}