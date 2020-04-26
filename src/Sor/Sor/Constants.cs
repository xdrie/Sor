using Nez;

namespace Sor {
    public static class Constants {
        // - Tags
        public static class Tags {
            // entities
            public const int WING = 0x2888884;
            public const int THING = 0x9973824;
        }

        // colliders
        public static class Colliders {
            public const int SHIP = 0x2342340;
            public const int WALL = 0x2398448;
            public const int THING = 0x9823429;
            public const int LANE = 0x8723847;
            public const int SHOOT = 0x3784234;
        }

        // - Game mechanics
        public static class Mechanics {
            public const float BOOST_COOLDOWN = 1f;
            public const float SHOOT_COOLDOWN = 1f;
            public const float CAPSULE_SIZE = 400;
            public const float SHOOT_COST_PER_KG = 200;
            public const float SHOOT_DRAIN = 2000;
            public const float CALORIES_PER_KG = 2f;
            public const int TRIGGER_GRAVITY = 0x9233742;
        }

        public static class Physics {
            // physics layers
            public const int LAYER_DEFAULT = 0;
            public const int LAYER_FIRE = 1 << 3;
            
            // birds
            
            // - defaults (wing)
            public const float DEF_MASS = 10f;
            public const float DEF_TURN_POWER = Mathf.PI * 0.72f;
            public const float DEF_THRUST_POWER = 120;
            public const float DEF_TOP_SPEED = 80f;
            public const float DEF_BOOST_FACTOR = 6.2f;
            public const float DEF_BOOST_TOP_SPEED = 400f;
            public const float DEF_BASE_DRAG = 16f;
            public const float DEF_BRAKE_DRAG = 80f;
            
            // alt (predator)
            public const float BIG_MASS = 80f;
            public const float BIG_TURN_POWER = Mathf.PI * 0.22f;
            public const float BIG_THRUST_POWER = 50f;
            public const float BIG_BOOST_TOP_SPEED = 200f;
            
            // alt (beak)
            public const float SML_MASS = 4f;
            public const float SML_TURN_POWER = Mathf.PI * 0.96f;
            public const float SML_THRUST_POWER = 210f;
            public const float SML_BOOST_TOP_SPEED = 640f;
        }

        public static class Game {
            public const string GAME_SLOT_0 = "game0.sav";
        }
    }
}