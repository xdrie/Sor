using Glint.Config;
using Glint.Util;

namespace Sor.Game {
    public class Config {
        public bool maxVfx = true; // enable all visual effects
        public bool fullscreen = false;
        public int framerate = 60;
        public int scaleMode = (int) ScaleMode.PixelPerfect;
        public int w = 960;
        public int h = 540;
        public int logLevel = (int) GlintLogger.LogLevel.Information;
        public string logFile = null;

        public enum ScaleMode {
            PixelPerfect,
            Stretch
        };

        public void read(string cf) {
            ConfigParser pr = new ConfigParser();
            pr.parse(cf);
            w = pr.getInt("video.w", w);
            h = pr.getInt("video.h", h);
            fullscreen = pr.getBool("video.fullscreen", fullscreen);
            scaleMode = pr.getInt("video.scaleMode", scaleMode);
            framerate = pr.getInt("video.framerate", framerate);
            maxVfx = pr.getBool("video.maxVfx", maxVfx);

            logLevel = pr.getInt("platform.logLevel", logLevel);
            logFile = pr.getStr("platform.logFile", logFile);
        }
    }
}