using System;
using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Components.Camera {
    public class LockedCamera : Component, IUpdatable {
        [Flags]
        public enum LockMode {
            Position,
            Rotation
        }

        private readonly Nez.Camera Camera;
        private readonly LockMode lockMode;
        private Vector2 _lastPosition;

        private Vector2 _precisePosition;

        public LockedCamera(Entity target, Nez.Camera Camera, LockMode lockMode) {
            this.target = target;
            this.Camera = Camera;
            this.lockMode = lockMode;
        }

        public Entity target { get; private set; }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            Entity.UpdateOrder = int.MaxValue;
        }

        public void setTarget(Entity target) {
            this.target = target;
        }

        public void Update() {
            if (target != null) updateFollow();
        }

        private void updateFollow() {
            // handle teleportation
            if (_lastPosition != Camera.Position) _precisePosition = Camera.Position;

            // lock position
            if (lockMode.HasFlag(LockMode.Position)) _precisePosition = target.Position;

            // lock rotation
            if (lockMode.HasFlag(LockMode.Rotation)) Camera.Transform.LocalRotation = -target.Transform.LocalRotation;

            Camera.Position = _precisePosition;

            _lastPosition = Camera.Position;
        }
    }
}