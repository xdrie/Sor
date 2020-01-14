using Nez;
using Sor.Components.Input;
using Sor.Components.Units;

namespace Sor.AI {
    /// <summary>
    /// represents the consciousness of a Wing
    /// </summary>
    public class Mind : Component, IUpdatable {
        public MindState state;
        public LogicInputController controller;
        public Wing me;

        public override void Initialize() {
            base.Initialize();

            controller = Entity.GetComponent<LogicInputController>();
            me = Entity.GetComponent<Wing>();
            
            state = new MindState(this);
        }

        public void Update() { // Sense-Think-Act
            sense(); // sense the world around
            think(); // think about the information available
            act(); // act on the information
        }

        private void act() {
            controller.zero(); // reset the controller
        }

        private void think() {
        }

        private void sense() {
        }
    }
}