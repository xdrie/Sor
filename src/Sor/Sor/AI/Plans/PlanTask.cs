using Nez;

namespace Sor.AI.Plans {
    public abstract class PlanTask {
        protected readonly Mind mind;
        public float failureTime = 0f;

        public PlanTask(Mind mind, float reachBefore) {
            this.mind = mind;
            failureTime = reachBefore;
        }
        
        public virtual bool valid() {
            if (failureTime <= 0) return true;
            return Time.TotalTime < failureTime;
        }
    }
}