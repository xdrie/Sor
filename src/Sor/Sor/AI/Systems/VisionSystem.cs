using System.Linq;
using System.Threading;
using Ducia.Layer1;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Game;

namespace Sor.AI.Systems {
    public class VisionSystem : DuckMindSystem {
        private Vector2 senseVec => new Vector2(Constants.DuckMind.SENSE_RANGE);
        public RectangleF sensorRec => new RectangleF(entity.Position - senseVec / 2, senseVec);

        public VisionSystem(DuckMind mind, float refresh, CancellationToken cancelToken) :
            base(mind, refresh, cancelToken) { }

        protected override void process() {
            // boxcast in radius
            var sensorCollResults = Physics.BoxcastBroadphase(sensorRec).ToList();
            var playContext = NGame.Services.GetService<PlayState>();

            state.clearVision();
            foreach (var sensorResult in sensorCollResults) {
                if (sensorResult.Entity == null) continue;
                var sensed = sensorResult.Entity;
                if (sensorResult.Tag == Constants.Colliders.SHIP && sensed != entity) {
                    if (NGame.context.config.invisible) {
                        if (sensed.Name == playContext.player.name) {
                            continue; // make player invisible
                        }
                    }

                    state.seenWings.Add(sensed.GetComponent<Wing>());
                }
                else if (sensorResult.Tag == Constants.Colliders.THING) {
                    state.seenThings.Add(sensed.GetComponent<Thing>());
                }
            }
        }
    }
}