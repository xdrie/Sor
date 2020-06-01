using Nez;

namespace Sor.AI.Plans {
    public abstract class PlanTask {
        protected readonly DuckMind mind;
        public float failureTime = 0f;

        public enum Status {
            /// <summary>
            /// task is still running
            /// </summary>
            Ongoing,
            /// <summary>
            /// task completed successfully
            /// </summary>
            Complete,
            /// <summary>
            /// task failed, but is optional
            /// </summary>
            OptionalFailed,
            /// <summary>
            /// task failed unrecoverably
            /// </summary>
            Failed
        }

        public PlanTask(DuckMind mind, float reachBefore) {
            this.mind = mind;
            failureTime = reachBefore;
        }
        
        /// <summary>
        /// whether the goal should still be pursued (valid/ongoing)
        /// </summary>
        /// <returns></returns>
        public virtual Status status() {
            if (failureTime <= 0) return Status.Ongoing;
            return Time.TotalTime < failureTime ? Status.Ongoing : Status.Failed;
        }
    }
}