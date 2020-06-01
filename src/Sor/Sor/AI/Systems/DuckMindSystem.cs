using System.Threading;
using Ducia;

namespace Sor.AI.Systems {
    public abstract class DuckMindSystem : MindSystem<DuckMind, DuckMindState> {
        protected DuckMindSystem(DuckMind mind, float refresh, CancellationToken cancelToken) : base(mind, refresh, cancelToken) { }
    }
}