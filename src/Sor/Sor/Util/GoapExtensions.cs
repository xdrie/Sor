using Activ.GOAP;

namespace Sor.Util {
    public static class GoapExtensions {
        public static bool matches<TModel>(this Node<TModel> node, string action) {
            return ((string) node.action) == action;
        }
    }
}