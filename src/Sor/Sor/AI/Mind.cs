using System.Linq;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI.Systems;
using Sor.Components.Input;
using Sor.Components.Things;
using Sor.Components.Units;

namespace Sor.AI {
    /// <summary>
    /// represents the consciousness of a Wing
    /// </summary>
    public class Mind : Component, IUpdatable {
        public MindState state;
        public LogicInputController controller;
        public Wing me;
        
        public VisionSystem visionSystem;

        public override void Initialize() {
            base.Initialize();

            controller = Entity.GetComponent<LogicInputController>();
            me = Entity.GetComponent<Wing>();

            state = new MindState(this);
            visionSystem = new VisionSystem(this, 0.2f);
        }

        public void Update() { // Sense-Think-Act
            sense(); // sense the world around
            think(); // think based on information and make plans
            act(); // carry out decisions
        }

        private void act() {
            controller.zero(); // reset the controller
        }

        private void think() { }

        private void sense() {
            visionSystem.tick(); // vision
        }
    }
}