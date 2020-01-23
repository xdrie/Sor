using Glint.Util;

namespace Sor {
    public static class Global {
        // TODO: configurable logging
        public static GlintLogger log = new GlintLogger(GlintLogger.LogLevel.Trace);
    }
}