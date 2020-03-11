using Activ.GOAP;

namespace Sor.AI.Model {
    public abstract class ActionPlanningModel<T> : Agent, Clonable<T> {
        public float cost { get; set; }
        private Option[] cachedOpts; // caching reduces array alloc overheads
        public Option[] Options() => cachedOpts ??= ActionOptions;
        public abstract Option[] ActionOptions { get; }
        public abstract T Allocate();
        public abstract T Clone(T storage);
    }
}