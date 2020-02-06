using Nez;

namespace Sor.AI.Model {
    public abstract class PlanTask {
        public float failureTime = 0f;

        public PlanTask(float reachBefore) {
            this.failureTime = reachBefore;
        }
        
        public virtual bool valid() {
            if (failureTime <= 0) return true;
            return Time.TotalTime < failureTime;
        }
    }
}