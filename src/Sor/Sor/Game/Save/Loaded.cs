using Glint.Util;

namespace Sor.Game.Save {
    public abstract class Loaded<T> where T : class {
        public T instance;

        public Loaded(T instance) {
            GAssert.Ensure(instance != null);
            this.instance = instance;
        }
    }
}