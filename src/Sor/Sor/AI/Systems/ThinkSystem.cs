using System.Threading;
using LunchtimeGears.AI.Utility;
using Sor.AI.Cogs.Interactions;
using Sor.AI.Consid;
using Sor.AI.Signals;

namespace Sor.AI.Systems {
    public class ThinkSystem : MindSystem {
        public int maxSignalsPerThink = 40;

        public ThinkSystem(Mind mind, float refresh, CancellationToken cancelToken) :
            base(mind, refresh, cancelToken) { }

        protected override void process() {
            // look at signals
            var processedSignals = 0;
            while (state.signalQueue.TryDequeue(out var signal)) {
                // process the signal
                processSignal(signal);

                processedSignals++;
                if (processedSignals > maxSignalsPerThink) break;
            }

            // update soul systems
            mind.soul.tick();

            // look at mind information
            thinkVisual();

            // figure out plans
            makePlans();
        }

        private void makePlans() {
            // create utility planner
            var reasoner = new Reasoner<Mind>();
            
            var eatConsideration = new ThresholdConsideration<Mind>(() => {
                // TODO: eat action
            }, 0.85f, "eat");
            eatConsideration.addAppraisal(new HungerAppraisals.HungerAppraisal(mind)); // 0-1
            eatConsideration.addAppraisal(new HungerAppraisals.FoodAvailabilityAppraisal(mind)); //0-1
            eatConsideration.scale = 1 / 2f;
            reasoner.addConsideration(eatConsideration);
            
            var exploreConsideration = new SumConsideration<Mind>(() => {
                // explore action
            }, "explore");
            exploreConsideration.addAppraisal(new ExploreAppraisals.ExplorationTendencyAppraisal(mind));
            exploreConsideration.addAppraisal(new ExploreAppraisals.UnexploredAppraisal(mind));
            exploreConsideration.scale = 1 / 2f;
            reasoner.addConsideration(exploreConsideration);

            var resultTable = reasoner.execute();
            state.lastPlanTable = resultTable;
            var chosen = reasoner.choose(resultTable);
        }

        private void processSignal(MindSignal result) {
            switch (result) {
                case ItemSignals.CapsuleAcquiredSignal sig: {
                    var from = sig.cap.sender;
                    if (from != null && from != mind.me) {
                        // run a feeding interaction
                        var interaction = new CapsuleFeedingInteraction(sig);
                        interaction.run(mind.soul, from.mind.soul);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Think about visual data available to me.
        /// </summary>
        private void thinkVisual() {
            // look at wings and their distances to me.
            lock (state.seenWings) {
                foreach (var wing in state.seenWings) {
                    var toWing = entity.Position - wing.Entity.Position;
                    var toWingDist = toWing.Length();
                    if (toWingDist <= NearbyBirdInteraction.nearRange) {
                        var interaction = new NearbyBirdInteraction(toWingDist);
                        interaction.run(mind.soul, wing.mind.soul);
                    }
                }
            }
        }
    }
}