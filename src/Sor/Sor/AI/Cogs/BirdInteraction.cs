using LunchtimeGears.Cogs;
using LunchtimeGears.Cogs.Social;
using LunchtimeGears.Units;

namespace Sor.AI.Cogs {
    public abstract class BirdInteraction : Interaction {
        public struct Traits {
            public float
                loyalty,
                aggression;

            public Traits(Person person) {
                loyalty = VectorTrait.value(BirdTraits.vec_loyalty, person.ply);
                aggression = VectorTrait.value(BirdTraits.vec_aggression, person.ply);
            }
        }
    }
}