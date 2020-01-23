using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using LunchtimeGears.Units;
using Nez;
using Sor.AI.Cogs;
using Sor.AI.Signals;
using Sor.AI.Systems;
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
        public VisionSystem visionSystem;
        public AvianSoul soul;

        public int maxSignalsPerThink = 40;

        public int consciousnessSleep = 100;
        protected Task consciousnessTask;
        protected CancellationTokenSource conciousnessCancel;

        public override void Initialize() {
            base.Initialize();

            controller = Entity.GetComponent<LogicInputController>();
            me = Entity.GetComponent<Wing>();

            state = new MindState(this);
            
            // mind systems
            var cts = new CancellationTokenSource();
            conciousnessCancel = cts;
            visionSystem = new VisionSystem(this, 0.2f, cts.Token);

            // start processing tasks
            consciousnessTask = consciousnessAsync(conciousnessCancel.Token);
        }

        public async Task consciousnessAsync(CancellationToken tok) {
            while (!tok.IsCancellationRequested) {
                think(); // think based on information and make plans
                await Task.Delay(consciousnessSleep, tok);
            }
        }

        public void Update() { // Sense-Think-Act AI
            sense(); // sense the world around
            act(); // carry out decisions
        }

        public override void OnRemovedFromEntity() {
            base.OnRemovedFromEntity();

            // stop processing tasks
            conciousnessCancel.Cancel();
        }

        private void act() {
            controller.zero(); // reset the controller
        }

        private void think() {
            // look at signals
            var processedSignals = 0;
            while (state.signalQueue.TryDequeue(out var signal)) {
                // process the signal
                processSignal(signal);

                processedSignals++;
                if (processedSignals > maxSignalsPerThink) break;
            }
        }

        private void processSignal(MindSignal result) {
            throw new System.NotImplementedException();
        }

        private void sense() {
            visionSystem.tick(); // vision
        }

        public void signal(MindSignal signal) {
            state.signalQueue.Enqueue(signal);
        }
    }
}