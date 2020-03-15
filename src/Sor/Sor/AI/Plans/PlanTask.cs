using Nez;

namespace Sor.AI.Plans {
    public abstract class PlanTask {
        public float failureTime = 0f;

        public PlanTask(float reachBefore) {
            failureTime = reachBefore;
        }
        
        public virtual bool valid() {
            if (failureTime <= 0) return true;
            return Time.TotalTime < failureTime;
        }
    }
}