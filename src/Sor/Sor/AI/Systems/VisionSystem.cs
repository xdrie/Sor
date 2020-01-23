using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Things;
using Sor.Components.Units;

namespace Sor.AI.Systems {
    public class VisionSystem : MindSystem {
        private Vector2 senseVec => new Vector2(MindConstants.SENSE_RANGE);
        public RectangleF sensorRec => new RectangleF(entity.Position - senseVec / 2, senseVec);

        public VisionSystem(Mind mind, float refresh, CancellationToken cancelToken) :
            base(mind, refresh, cancelToken) { }

        protected override void process() {
            // boxcast in radius
            var sensorCollResults = Physics.BoxcastBroadphase(sensorRec).ToList();
            lock (state.seenWings)
            lock (state.seenThings) {
                state.seenWings.Clear();
                state.seenThings.Clear();
                foreach (var sensorResult in sensorCollResults) {
                    if (sensorResult.Entity == null) continue;
                    var sensed = sensorResult.Entity;
                    if (sensorResult.Tag == Constants.COLLIDER_SHIP && sensed != entity) {
                        state.seenWings.Add(sensed.GetComponent<Wing>());
                    } else if (sensorResult.Tag == Constants.COLLIDER_THING) {
                        state.seenThings.Add(sensed.GetComponent<Thing>());
                    }
                }
            }
        }
    }
}