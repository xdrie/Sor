using Glint.AI.Misc;

namespace Sor.AI.Cogs {
    /// <summary>
    /// Personality math utilities
    /// </summary>
    public static class PerMath {
        /// <summary>
        ///     Given v, take (min * v) and add ((max-min) * d)
        ///     Useful for scaling a value between a min and max value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="d"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float twoSegment(float v, float d, float min, float max, bool clamp = false) {
            var s1 = v * min;
            var b = max - min;
            var r = s1 + b * d;
            if (clamp) r = Gmathf.clamp(r, min, max);
            return r;
        }
    }
}