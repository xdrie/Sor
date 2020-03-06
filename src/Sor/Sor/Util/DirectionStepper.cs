using Nez;

namespace Sor.Util {
    public static class DirectionStepper {
        public static (int, int) stepIn(Direction direction) {
            var dx = 0;
            var dy = 0;
            switch (direction) {
                case Direction.Up:
                    dy = -1;
                    break;
                case Direction.Right:
                    dx = 1;
                    break;
                case Direction.Down:
                    dy = 1;
                    break;
                case Direction.Left:
                    dx = -1;
                    break;
            }
            return (dx, dy);
        }
    }
}