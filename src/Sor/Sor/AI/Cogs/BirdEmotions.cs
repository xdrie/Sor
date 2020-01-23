namespace Sor.AI.Cogs {
    public class BirdEmotions {
        public const float falloff = 0.9f;
        
        // emotions: [fear, happy]
        public float[] emotions = {0f, 0f};

        public float fear {
            get { return emotions[0]; }
            set { emotions[0] = value; }
        }
        
        public float happy {
            get { return emotions[1]; }
            set { emotions[1] = value; }
        }

        public void tick() {
            // exponentially tend emotions toward zero
            for (var i = 0; i < emotions.Length; i++) {
                emotions[i] = falloff * emotions[i];
            }
        }
    }
}