using Glint.Physics;

namespace Sor.Components.Units {
    public class ShipBody : KinematicBody {
        public Ship me;

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            
            me = Entity.GetComponent<Ship>();
        }

        public override void Update() {
            base.Update();
        }
    }
}