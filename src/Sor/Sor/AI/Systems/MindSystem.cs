using Nez;

namespace Sor.AI.Systems {
    /// <summary>
    /// Represents a system of a mind with a refresh rate
    /// </summary>
    public abstract class MindSystem {
        public Mind mind;
        protected MindState state;
        public float refresh;
        public float nextRefreshAt;
        protected Entity entity;
        
        public MindSystem(Mind mind, float refresh) {
            this.mind = mind;
            state = mind.state;
            this.entity = mind.Entity;
            this.refresh = refresh;
        }

        protected abstract void process();

        /// <summary>
        /// Ticks the mind, calling process if it is time.
        /// </summary>
        /// <returns>Whether process was called.</returns>
        public virtual bool tick() {
            if (Time.TotalTime > nextRefreshAt) {
                nextRefreshAt = Time.TotalTime + refresh;
                process();
                return true;
            }

            return false;
        }
    }
}