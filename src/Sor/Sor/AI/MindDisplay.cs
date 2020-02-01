using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Nez;
using Sor.Components.Units;

namespace Sor.AI {
    public class MindDisplay : RenderableComponent, IUpdatable {
        private Wing player;
        private Mind mind;
        private Wing wing;
        private Color textCol = Core.Services.GetService<GameContext>().assets.fgColor;
        private bool draw;

        public MindDisplay(Wing player, bool draw) {
            this.player = player;
            this.draw = draw;
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            mind = Entity.GetComponent<Mind>();
            mind.debug = true; // enable trace debug
            wing = mind.me;
        }

        public override void OnRemovedFromEntity() {
            base.OnRemovedFromEntity();

            mind.debug = false; // disable trace debug
        }

        public override RectangleF Bounds {
            get { return Entity.Scene.Camera.Bounds; }
        }

        public override void Render(Batcher batcher, Camera camera) {
            if (draw) {
                // TODO: show information about whose mind, etc.
                // draw mind info representation
                var plOpinion = mind.state.getOpinion(player.mind);

                StringBuilder ind = new StringBuilder();

                // draw basic mind state
                ind.AppendLine($"[mind] {wing.name}");
                ind.AppendLine($"energy: {wing.core.ratio:n2}");
                ind.AppendLine($"vision: {mind.state.seenWings.Count} | {mind.state.seenThings.Count}");
                ind.AppendLine($"opinion: {plOpinion} | {opinionTag(plOpinion)}");
                ind.AppendLine($"prsntly: {mind.soul.ply}");
                ind.AppendLine($"emo: H:{mind.soul.emotions.happy:n2}, F:{mind.soul.emotions.fear:n2}");

                // draw plan table
                // TODO: draw arrow in front of chosen
                if (mind.state.lastPlanTable != null) {
                    lock (mind.state.lastPlanTable) {
                        foreach (var consid in mind.state.lastPlanTable) {
                            ind.Append($"({consid.Key.tag}: {consid.Value:n2}) ");
                        }
                    }

                    ind.AppendLine();
                }

                batcher.DrawString(Graphics.Instance.BitmapFont, ind,
                    camera.ScreenToWorldPoint(new Vector2(20, 20)), textCol);
            }
        }

        private string opinionTag(int opinion) {
            if (opinion <= MindConstants.OPINION_DESPISE) {
                return "despise";
            } else if (opinion <= MindConstants.OPINION_HATE && opinion > MindConstants.OPINION_DESPISE) {
                return "hate";
            } else if (opinion > MindConstants.OPINION_HATE && opinion < MindConstants.OPINION_ALLY) {
                return "wary"; // in the middle: wary
            } else if (opinion >= MindConstants.OPINION_ALLY && opinion < MindConstants.OPINION_FRIEND) {
                return "ally";
            } else if (opinion >= MindConstants.OPINION_FRIEND && opinion < MindConstants.OPINION_KIN) {
                return "friend";
            } else if (opinion >= MindConstants.OPINION_KIN) {
                return "kin";
            } else {
                return "wat";
            }
        }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);

            // sensor rect
            batcher.DrawHollowRect(new Rectangle(mind.visionSystem.sensorRec.Location.ToPoint(),
                mind.visionSystem.sensorRec.Size.ToPoint()), Color.Green);
        }

        public void Update() {
            // TODO: update summary info
        }
    }
}