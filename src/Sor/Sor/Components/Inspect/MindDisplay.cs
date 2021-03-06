using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ducia;
using Ducia.Framework.Utility.Considerations;
using Ducia.Game;
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
        private DuckMind mind;
        private Wing wing;
        private Color textCol = Core.Services.GetService<GameContext>().assets.fgColor;
        private bool draw;

        public MindDisplay(Wing player, bool draw) {
            this.player = player;
            this.draw = draw;
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            wing = Entity.GetComponent<Wing>();
            mind = wing.mind;
            mind.inspected = true; // enable trace debug
        }

        public override void OnRemovedFromEntity() {
            base.OnRemovedFromEntity();

            mind.inspected = false; // disable trace debug
        }

        public override RectangleF Bounds =>
            new RectangleF(0, 0, Entity.Scene.DesignResolution.X, Entity.Scene.DesignResolution.Y);

        void drawIndicator(Batcher batcher, Vector2 pos, Color col, float size = 4f, float thickness = 1f) {
            var drawPos = Entity.Scene.Camera.WorldToScreenPoint(pos);
            batcher.DrawHollowRect(
                new RectangleF(drawPos.X - size, drawPos.Y - size, size * 2, size * 2),
                col, thickness);
        }

        public override void Render(Batcher batcher, Camera camera) {
            if (draw) {
                // draw mind info representation
                // highlight the inspected wing
                drawIndicator(batcher, mind.state.me.body.pos, Color.White, 8f);

                // text container for all display text
                var ind = new ColoredTextBuilder(Color.White);

                // draw basic mind state
                ind.appendLine($"[mind] {wing.name}");
                ind.appendLine($"energy: {wing.core.ratio:n2}");
                ind.appendLine($"vision: {mind.state.seenWings.Count} | {mind.state.seenThings.Count}");
                if (player != null) {
                    var plOpinion = mind.state.getOpinion(player.mind.state.me);
                    ind.appendLine($"opinion: {plOpinion} | {opinionTag(plOpinion)}");
                }

                var opinionTable = mind.state.opinion.ToList();
                var positive = 0;
                var negative = 0;
                var netOpi = 0;
                foreach (var op in opinionTable) {
                    var opVal = op.Value;
                    var pos = opVal > Constants.DuckMind.OPINION_NEUTRAL;
                    netOpi += opVal;
                    if (pos) {
                        positive++;
                    }
                    else {
                        negative++;
                    }
                }

                ind.appendLine($"rel: +{positive} | -{negative} = {netOpi}");

                ind.appendLine($"ply: {mind.soul.ply}");
                ind.appendLine($"emo: H:{mind.soul.emotions.happy:n2}, F:{mind.soul.emotions.fear:n2}");

                // draw plan table
                var first = false;
                var optionScores = mind.state.lastPlanLog.ToArray()
                    .OrderByDescending(x => x.Value)
                    .ToList();

                ind.appendLine("reasoner");
                foreach (var consid in optionScores) {
                    var considSb = new StringBuilder();
                    // exclude zeroes
                    // if (consid.Value.Approximately(0)) continue;
                    if (!first) {
                        considSb.Append(" > ");
                        first = true;
                    }
                    else {
                        considSb.Append("   ");
                    }

                    // add consid nam and score
                    considSb.Append($"{consid.Key.tag}: {consid.Value:n2}"); // add consid: score
#if DEBUG
                    if (SorDebug.aiTrace) {
                        // add appraisals
                        foreach (var appr in consid.Key.lastScores.ToList()) {
                            var lowerCamelName = appr.Key.GetType().Name;
                            var nameBuilder = new StringBuilder();
                            nameBuilder.Append(lowerCamelName[0].ToString().ToLower());
                            nameBuilder.Append(lowerCamelName.Substring(1));
                            var apprName = StringUtils.abbreviate(nameBuilder.ToString(), 2);
                            considSb.Append($" ({apprName}: {appr.Value:n2})");
                        }
                    }
#endif

                    considSb.AppendLine();

                    ind.appendLine(considSb.ToString());
                }

                // attempt to illustrate the plans
                var planItems = mind.state.plan.ToList();
                var planSb = new StringBuilder();
                var planAhead = NGame.config.mindDisplayAhead;
                planSb.AppendLine($"plan [{mind.state.plan.Count}]");
                var nextInPlan = mind.state.plan.Take(planAhead);
                foreach (var planTask in nextInPlan) {
                    if (planTask.status() == PlanTask.Status.Ongoing) {
                        switch (planTask) {
                            case TargetSource target: {
                                // illustrate the target and objective
                                
                                var targetType = target.GetType().Name;
                                var targetPos = target.getPosition();
                                var approachPos = target.approachPosition();
                                var secondaryPos = target.getSecondaryPosition();
                                planSb.Append($" [T]{targetType}: ({targetPos.X:n1}, {targetPos.Y:n1})");
                                var targetColor = Color.Yellow; // color of target indicator

                                // add extra annotation if target is wing
                                if (target is EntityTarget ets && ets.nt.HasComponent<Wing>()) {
                                    planSb.Append($" {ets.nt.Name}");
                                    var opinion = mind.state.getOpinion(ets.nt.GetComponent<Wing>().mind.state.me);
                                    var (_, displayColor) = PipsSystem.calculatePips(opinion);
                                    targetColor = displayColor;
                                }

                                // annotation for approach type
                                if (target.approachRange > 0) {
                                    planSb.Append($" [r={target.approachRange:n2}]");
                                }

                                planSb.AppendLine();

                                drawIndicator(batcher, targetPos, targetColor, 4f);
                                drawIndicator(batcher, approachPos, Color.LightBlue, 4f);
                                if (secondaryPos != null)
                                    drawIndicator(batcher, secondaryPos.Value, Color.Cyan, 4f);
                                break;
                            }
                            case SingleInteraction<DuckMind> inter:
                                planSb.AppendLine($" {inter.GetType().Name} {inter.target.Name}");
                                break;
                        }
                    }
                }

                ind.appendLine(planSb.ToString());

                ind.appendLine();
                // draw board
                var boardItems = mind.state.board.ToList();
                // sort board items by tag
                var orderedBoardItems =
                    boardItems.OrderBy(x => x.Value.tag)
                        .ToArray();
                var taggedItems = new Dictionary<string, List<(string, DuckMindState.BoardItem)>>();
                foreach (var groupedBoardItem in orderedBoardItems) {
                    if (!taggedItems.ContainsKey(groupedBoardItem.Value.tag)) {
                        taggedItems[groupedBoardItem.Value.tag] = new List<(string, DuckMindState.BoardItem)>();
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

                ind.drawTo(batcher, Graphics.Instance.BitmapFont, new Vector2(20, 20));
            }
        }

        private string opinionTag(int opinion) {
            if (opinion <= Constants.DuckMind.OPINION_DESPISE) {
                return "despise";
            }
            else if (opinion <= Constants.DuckMind.OPINION_HATE && opinion > Constants.DuckMind.OPINION_DESPISE) {
                return "hate";
            }
            else if (opinion > Constants.DuckMind.OPINION_HATE && opinion < Constants.DuckMind.OPINION_ALLY) {
                return "wary"; // in the middle: wary
            }
            else if (opinion >= Constants.DuckMind.OPINION_ALLY && opinion < Constants.DuckMind.OPINION_FRIEND) {
                return "ally";
            }
            else if (opinion >= Constants.DuckMind.OPINION_FRIEND && opinion < Constants.DuckMind.OPINION_KIN) {
                return "friend";
            }
            else if (opinion >= Constants.DuckMind.OPINION_KIN) {
                return "kin";
            }
            else {
                return "wat";
            }
        }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);

            // // sensor rect
            // batcher.DrawHollowRect(new Rectangle(mind.visionSystem.sensorRec.Location.ToPoint(),
            //     mind.visionSystem.sensorRec.Size.ToPoint()), Color.Green);
        }

        public void Update() {
            // TODO: update summary info
        }
    }
}