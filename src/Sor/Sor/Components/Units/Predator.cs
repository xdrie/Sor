using Nez;
using Sor.AI;

namespace Sor.Components.Units {
    public class Predator : Wing {
        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            
            Transform.SetLocalScale(2f);
            pips.spriteRenderer.LocalOffset = pips.spriteRenderer.LocalOffset * 2f;
            
            body.turnPower = Mathf.PI * 0.22f;
            body.thrustPower = 1f;
            body.mass = 80f;
            body.recalculateKinematics();
        }

        public Predator(Mind mind) : base(mind) { }
    }
}