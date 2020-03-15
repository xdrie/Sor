using LunchLib.Cogs;
using Sor.AI.Signals;
using Sor.Components.Units;

namespace Sor.AI.Cogs.Interactions {
    public class ShotInteraction : BirdInteraction {
        public Wing shooter;
        public PhysicalSignals.ShotSignal sig;
        
        struct Traits {
            public static float[] vec_anger = {0.6f, 0.9f};
            public float anger;

            public Traits(AvianSoul soul) {
                var tr = soul.traits;
                anger = VectorTrait.trait2(vec_anger, new[] {
                    tr.wary, tr.aggression
                });
            }
        }

        public ShotInteraction(Wing shooter, PhysicalSignals.ShotSignal sig) {
            this.shooter = shooter;
            this.sig = sig;
        }

        public override void runTwo(AvianSoul me, AvianSoul them) {
            // i was shot by them
            var myTraits = new Traits(me);

            // we are ANGERY
            // TODO: calculate anger amount from being shot
            me.mind.state.addOpinion(them.mind, -40);
        }
    }
}