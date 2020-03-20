using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glint.Util;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI;
using Sor.AI.Plans;
using Sor.Components.Units;
using Sor.Game;
using Sor.Systems;
using Sor.Util;

namespace Sor.Components.Inspect {
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
            mind.inspected = true; // enable trace debug
            wing = mind.me;
        }

        public override void OnRemovedFromEntity() {
            base.OnRemovedFromEntity();

            mind.inspected = false; // disable trace debug
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

                lock (mind.state.opinion) {
                    var positive = 0;
                    var negative = 0;
                    var netOpi = 0;
                    foreach (var op in mind.state.opinion) {
                        var opVal = op.Value;
                        var pos = opVal > MindConstants.OPINION_NEUTRAL;
                        netOpi += opVal;
                        if (pos) {
                            positive++;
                        } else {
                            negative++;
                        }
                    }

                    ind.appendLine($"rel: +{positive} | -{negative} = {netOpi}");
                }

                ind.appendLine($"ply: {mind.soul.ply}");
                ind.appendLine($"emo: H:{mind.soul.emotions.happy:n2}, F:{mind.soul.emotions.fear:n2}");

                // draw plan table
                // TODO: draw arrow in front of chosen
                if (mind.state.lastPlanLog != null) {
                    lock (mind.state.lastPlanLog) {
                        var first = false;
                        var options = mind.state.lastPlanLog
                            .OrderByDescending(x => x.Value).ToList();
                        foreach (var consid in options) {
                            // exclude zeroes
                            // if (consid.Value.Approximately(0)) continue;
                            var sb = new StringBuilder();
                            if (!first) {
                                sb.Append("> ");
                                first = true;
                            } else {
                                sb.Append("  ");
                            }

                            // add consid nam and score
                            sb.Append($"{consid.Key.tag}: {consid.Value:n2}"); // add consid: score
#if DEBUG
                            if (SorDebug.aiTrace) {
                                // add appraisals
                                foreach (var appr in consid.Key.lastScores) {
                                    var lowerCamelName = appr.Key.GetType().Name;
                                    var nameBuilder = new StringBuilder();
                                    nameBuilder.Append(lowerCamelName[0].ToString().ToLower());
                                    nameBuilder.Append(lowerCamelName.Substring(1));
                                    var apprName = StringUtils.abbreviate(nameBuilder.ToString(), 2);
                                    sb.Append($" ({apprName}: {appr.Value:n2})");
                                }
                            }
#endif

                            sb.AppendLine();

                            ind.appendLine(sb.ToString());
                        }
                    }
                }

                void drawPosIndicator(Vector2 pos, Color col) {
                    // draw indicator
                    var indSize = 4f;
                    batcher.DrawHollowRect(
                        new RectangleF(pos.X - indSize, pos.Y - indSize, indSize * 2, indSize * 2),
                        col, 1f);
                }

                lock (mind.state.plan) {
                    var sb = new StringBuilder();
                    var planAhead = 2;
                    var nextInPlan = mind.state.plan.Take(planAhead);
                    foreach (var planTask in nextInPlan) {
                        if (planTask.valid()) {
                            switch (planTask) {
                                case TargetSource target: {
                                    var targetLoc = target.getPosition();
                                    var approachLoc = target.approachPosition(mind.me.body.pos);
                                    sb.Append($" Target: ({targetLoc.X:n1}, {targetLoc.Y:n1})");
                                    var targetColor = Color.Yellow; // color of target indicator
                                    
                                    // add extra annotation if target is entity
                                    if (target is EntityTarget ets) {
                                        sb.Append($" {ets.nt.Name}");
                                        var opinion = mind.state.getOpinion(ets.nt.GetComponent<Wing>().mind);
                                        var (_, disp) = PipsSystem.calculatePips(opinion);
                                        targetColor = disp;
                                    }

                                    sb.AppendLine();

                                    drawPosIndicator(targetLoc, targetColor);
                                    drawPosIndicator(approachLoc, Color.LightBlue);
                                    break;
                                }
                                case SingleInteraction inter:
                                    sb.AppendLine($" {inter.GetType().Name} {inter.target.Name}");
                                    break;
                            }
                        }
                    }

                    ind.appendLine(sb.ToString());
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