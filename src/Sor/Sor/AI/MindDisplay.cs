using Microsoft.Xna.Framework;
using Nez;

namespace Sor.AI {
    public class MindDisplay : RenderableComponent, IUpdatable {
        private Mind mind;

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            mind = Entity.GetComponent<Mind>();
        }

        public override RectangleF Bounds {
            get {
                return Entity.Scene.Camera.Bounds;
            }
        }

        public override void Render(Batcher batcher, Camera camera) {
            // TODO: show information about whose mind, etc.
            // TODO: draw the info
            batcher.DrawString(Graphics.Instance.BitmapFont, "yeet", 
                camera.ScreenToWorldPoint(new Vector2(20, 20)), Color.White);
        }

        public void Update() {
            // TODO: update summary info
        }
    }
}