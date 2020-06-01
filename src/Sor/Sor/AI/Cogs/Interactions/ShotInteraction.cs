using Ducia.Calc;
using Ducia.Cogs;
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
            var myTraits = new Traits(me);

            var opinionDelta = 0;

            // we are angry, figure out how angry
            var beingShotAnger = (int) TraitCalc.transform(-myTraits.anger, 
                -40f, -5f, -40, 0f);
            // fear exacerbates anger/annoyance
            var fearMultiplier = TraitCalc.transform(me.emotions.fear,
                -1f, 3f, 0f, 2f);

            opinionDelta += (int) (beingShotAnger * fearMultiplier);

            me.mind.state.addOpinion(them.mind, opinionDelta);
            me.emotions.spikeFear(1); // scary
        }
    }
}