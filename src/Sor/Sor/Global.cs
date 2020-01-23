using Glint.Util;

namespace Sor {
    public static class Global {
        // TODO: configurable logging
        public static GlintLogger log;

        static Global() {
            log = new GlintLogger(GlintLogger.LogLevel.Trace);
            log.sinks.Add(new GlintLogger.ConsoleSink());
        }
    }
}