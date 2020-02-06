using System.Linq;
using System.Text;
using Glint.AI.Misc;
using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using Sor.AI.Model;
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
                // draw mind info representation

                StringBuilder ind = new StringBuilder();

                // draw basic mind state
                ind.AppendLine($"[mind] {wing.name}");
                ind.AppendLine($"energy: {wing.core.ratio:n2}");
                ind.AppendLine($"vision: {mind.state.seenWings.Count} | {mind.state.seenThings.Count}");
                if (player != null) {
                    var plOpinion = mind.state.getOpinion(player.mind);
                    ind.AppendLine($"opinion: {plOpinion} | {opinionTag(plOpinion)}");
                }

                ind.AppendLine($"ply: {mind.soul.ply}");
                ind.AppendLine($"emo: H:{mind.soul.emotions.happy:n2}, F:{mind.soul.emotions.fear:n2}");

                // draw plan table
                // TODO: draw arrow in front of chosen
                if (mind.state.lastPlanTable != null) {
                    lock (mind.state.lastPlanTable) {
                        var first = false;
                        foreach (var consid in mind.state.lastPlanTable.OrderByDescending(x => x.Value)) {
                            if (!first) {
                                ind.Append("> ");
                                first = true;
                            } else {
                                ind.Append("  ");
                            }

                            ind.AppendLine($"{consid.Key.tag}: {consid.Value:n2}");
                        }
                    }
                }

                lock (mind.state.targetQueue) {
                    if (mind.state.targetQueue.Count > 0) {
                        var target = mind.state.targetQueue.Peek();
                        if (target.valid()) {
                            var targetLoc = target.getPosition();
                            ind.Append($"tgt: ({targetLoc.X:n1}, {targetLoc.Y:n1})");
                            if (target is EntityTargetSource ets) {
                                ind.Append($" {ets.nt.Name}");
                            }

                            // draw indicator
                            var indSize = 4f;
                            var trackCol = new Color(150 + Nez.Random.NextInt(155), 150 + Nez.Random.NextInt(155), 0);
                            batcher.DrawHollowRect(
                                new RectangleF(targetLoc.X - indSize, targetLoc.Y - indSize, indSize * 2, indSize * 2),
                                trackCol, 1f);

                            ind.AppendLine();
                        }
                    }
                }

                ind.AppendLine();
                // draw board
                lock (mind.state.board) {
                    foreach (var kv in mind.state.board) {
                        ind.AppendLine($"  {kv.Key}: {kv.Value.v}");
                    }
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