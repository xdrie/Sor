using System.Threading;
using System.Threading.Tasks;
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
        public bool control; // whether the mind is in control
        public MindState state;
        public LogicInputController controller;
        public Wing me;
        public VisionSystem visionSystem;
        public ThinkSystem thinkSystem;
        public AvianSoul soul;

        public int consciousnessSleep = 100;
        protected Task consciousnessTask;
        protected CancellationTokenSource conciousnessCancel;

        public Mind() : this(true) { }

        public Mind(bool control) {
            this.control = control;
        }

        public override void Initialize() {
            base.Initialize();
            
            me = Entity.GetComponent<Wing>();
            state = new MindState(this);

            if (control) {
                // input
                controller = Entity.GetComponent<LogicInputController>();
                
                // mind systems
                var cts = new CancellationTokenSource();
                conciousnessCancel = cts;
                visionSystem = new VisionSystem(this, 0.2f, cts.Token);

                thinkSystem = new ThinkSystem(this, 0.2f, cts.Token);

                // start processing tasks
                consciousnessTask = consciousnessAsync(conciousnessCancel.Token);
            }
        }

        public async Task consciousnessAsync(CancellationToken tok) {
            while (!tok.IsCancellationRequested) {
                think(); // think based on information and make plans
                await Task.Delay(consciousnessSleep, tok);
            }
        }

        public void Update() { // Sense-Think-Act AI
            if (control) {
                sense(); // sense the world around
                act(); // carry out decisions
            }
        }

        public override void OnRemovedFromEntity() {
            base.OnRemovedFromEntity();

            if (control) {
                // stop processing tasks
                conciousnessCancel.Cancel();
            }
        }

        private void act() {
            controller.zero(); // reset the controller
        }

        private void think() {
            thinkSystem.tick();
        }

        private void sense() {
            visionSystem.tick(); // vision
        }

        public void signal(MindSignal signal) {
            state.signalQueue.Enqueue(signal);
        }
    }
}