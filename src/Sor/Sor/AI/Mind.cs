using System.Linq;
using Microsoft.Xna.Framework;
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
        
        private Vector2 senseVec => new Vector2(MindConstants.SENSE_RANGE);
        public RectangleF sensorRec => new RectangleF(Entity.Position - senseVec / 2, senseVec);

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

        private void think() { }

        private void sense() {
            // - vision
            // boxcast in radius
            var sensorCollResults = Physics.BoxcastBroadphase(sensorRec);
            state.detectedWings = sensorCollResults
                .Where(x => x.Tag == Constants.TAG_SHIP_COLLIDER && x.Entity != null && x.Entity != Entity)
                .Select(x => x.Entity.GetComponent<Wing>())
                .ToList();
        }
    }
}