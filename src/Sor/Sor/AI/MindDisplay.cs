using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Units;

namespace Sor.AI {
    public class MindDisplay : RenderableComponent, IUpdatable {
        private Mind mind;
        private Wing wing; 

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            mind = Entity.GetComponent<Mind>();
            wing = mind.me;
        }

        public override RectangleF Bounds {
            get {
                return Entity.Scene.Camera.Bounds;
            }
        }

        public override void Render(Batcher batcher, Camera camera) {
            // TODO: show information about whose mind, etc.
            // TODO: draw the info
            StringBuilder ind = new StringBuilder();
            ind.AppendLine($"[mind] {wing.name}");
            ind.AppendLine($"seen wings: {mind.state.detectedWings.Count}");
            batcher.DrawString(Graphics.Instance.BitmapFont, ind, 
                camera.ScreenToWorldPoint(new Vector2(20, 20)), Color.White);
            
            batcher.DrawHollowRect(new Rectangle(mind.sensorRec.Location.ToPoint(), mind.sensorRec.Size.ToPoint()), Color.Green);
        }

        public void Update() {
            // TODO: update summary info
        }
    }
}