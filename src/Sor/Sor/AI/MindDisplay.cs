using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using Sor.AI.Model;
using Sor.Components.Units;
using Sor.Util;

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

                var ind = new ColoredTextBuilder(Color.White);

                // draw basic mind state
                ind.appendLine($"[mind] {wing.name}");
                ind.appendLine($"energy: {wing.core.ratio:n2}");
                ind.appendLine($"vision: {mind.state.seenWings.Count} | {mind.state.seenThings.Count}");
                if (player != null) {
                    var plOpinion = mind.state.getOpinion(player.mind);
                    ind.appendLine($"opinion: {plOpinion} | {opinionTag(plOpinion)}");
                }

                ind.appendLine($"ply: {mind.soul.ply}");
                ind.appendLine($"emo: H:{mind.soul.emotions.happy:n2}, F:{mind.soul.emotions.fear:n2}");

                // draw plan table
                // TODO: draw arrow in front of chosen
                if (mind.state.lastPlanTable != null) {
                    lock (mind.state.lastPlanTable) {
                        var first = false;
                        foreach (var consid in mind.state.lastPlanTable.OrderByDescending(x => x.Value)) {
                            var sb = new StringBuilder();
                            if (!first) {
                                sb.Append("> ");
                                first = true;
                            } else {
                                sb.Append("  ");
                            }

                            sb.AppendLine($"{consid.Key.tag}: {consid.Value:n2}");
                            ind.appendLine(sb.ToString());
                        }
                    }
                }

                lock (mind.state.plan) {
                    if (mind.state.plan.Count > 0) {
                        var planTask = mind.state.plan.Peek();
                        if (planTask is TargetSource target) {
                            if (target.valid()) {
                                void drawPosIndicator(Vector2 pos, Color col) {
                                    // draw indicator
                                    var indSize = 4f;
                                    batcher.DrawHollowRect(
                                        new RectangleF(pos.X - indSize, pos.Y - indSize, indSize * 2, indSize * 2),
                                        col, 1f);
                                }

                                var targetLoc = target.getPosition();
                                var approachLoc = target.approachPosition(mind.me.body.pos);
                                var sb = new StringBuilder();
                                sb.Append($"tgt: ({targetLoc.X:n1}, {targetLoc.Y:n1})");
                                if (target is EntityTargetSource ets) {
                                    sb.Append($" {ets.nt.Name}");
                                }

                                ind.appendLine(sb.ToString());

                                // var trackCol = new Color(150 + Nez.Random.NextInt(155), 150 + Nez.Random.NextInt(155), 0);
                                drawPosIndicator(targetLoc, Color.Yellow);
                                drawPosIndicator(approachLoc, Color.Blue);

                                ind.appendLine();
                            }
                        }
                    }
                }

                ind.appendLine();
                // draw board
                lock (mind.state.board) {
                    var boardItems = mind.state.board;
                    // sort board items by tag
                    var orderedBoardItems =
                        boardItems.OrderBy(x => x.Value.tag)
                            .ToArray();
                    var taggedItems = new Dictionary<string, List<(string, MindState.BoardItem)>>();
                    foreach (var groupedBoardItem in orderedBoardItems) {
                        if (!taggedItems.ContainsKey(groupedBoardItem.Value.tag)) {
                            taggedItems[groupedBoardItem.Value.tag] = new List<(string, MindState.BoardItem)>();
                        }

                        taggedItems[groupedBoardItem.Value.tag]
                            .Add((groupedBoardItem.Key, groupedBoardItem.Value));
                    }

                    ind.appendLine("-- BOARD --");
                    foreach (var tagGroup in taggedItems) {
                        ind.appendLine($" {tagGroup.Key}");
                        foreach (var (key, boardItem) in tagGroup.Value) {
                            ind.appendLine($"  {key}: {boardItem.value}", boardItem.col);
                        }
                    }
                }

                ind.drawTo(batcher, Graphics.Instance.BitmapFont,
                    camera.ScreenToWorldPoint(new Vector2(20, 20)));
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