using Glint.Physics;
using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Components.Units {
    public class KinBody : PhysicsBody, IUpdatable {
        public Vector2 maxVelocity;
        public Vector2 acceleration;
        public Vector2 drag;
        public Vector2 pos {
            get { return Transform.LocalPosition; }
            set { Transform.LocalPosition = value; }
        }
        
        public float maxAngular;
        public float angularVelocity;
        public float angularAcceleration;
        public float angularDrag;

        public float angle {
            get { return Transform.LocalRotation; }
            set { Transform.LocalRotation = value; }
        }
        
        
        public virtual void Update() {
            float dt = Time.DeltaTime;
            
            velocity += acceleration * dt;
            var vls = velocity.LengthSquared();
            var mvls = maxVelocity.LengthSquared();
            if (vls > mvls) {
                // convert to unit and rescale
                var unitVel = Vector2Ext.Normalize(velocity);
                var ratio = mvls / vls;
                var reductionFac = Mathf.Pow(ratio, (1 / 12f));
                // smoothly reduce to max velocity
                velocity *= reductionFac;
            }

            if (velocity.X > drag.X * dt) {
                velocity.X -= drag.X * dt;
            }
            if (velocity.X < -drag.X * dt) {
                velocity.X += drag.X * dt;
            }
            if (velocity.Y > drag.Y * dt) {
                velocity.Y -= drag.Y * dt;
            }
            if (velocity.Y < -drag.Y * dt) {
                velocity.Y += drag.Y * dt;
            }
            pos += motion(velocity * dt);

            angularVelocity += angularAcceleration * dt;
            if (angularVelocity > maxAngular) {
                angularVelocity = maxAngular;
            }
            if (angularVelocity < -maxAngular) {
                angularVelocity = -maxAngular;
            }

            if (angularVelocity > angularDrag * dt) {
                angularVelocity -= angularDrag * dt;
            }
            if (angularVelocity < -angularDrag * dt) {
                angularVelocity += angularDrag * dt;
            }
            angle += angularVelocity * dt;
        }

        protected virtual Vector2 motion(Vector2 posDelta) {
            return posDelta;
        }
    }
}