using System;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI;
using Sor.Components.UI;
using Sor.Components.Units;

namespace Sor.Systems {
    public class PipsSystem : EntityProcessingSystem {
        private Wing player;

        public PipsSystem(Wing player) : base(new Matcher().All(typeof(Mind))) {
            this.player = player;
        }

        public override void Process(Entity entity) {
            // check if wing is visible to player
            var wing = entity.GetComponent<Wing>();
            if (wing == player) {
                player.pips.setPips(1, Pips.green);
            } else {
                if (wing.spriteRenderer.IsVisibleFromCamera(Scene.Camera)) {
                    // update pips relative to player
                    wing.pips.enable();
                    // get opinion of player
                    var playerOpinion = wing.mind.state.getOpinion(player.mind);
                    (var pipCount, var pipColor) = calculatePips(playerOpinion);
                    if (pipCount > 5) pipCount = 5;
                    wing.pips.setPips(pipCount, pipColor);
                } else {
                    wing.pips.disable();
                }
            }
        }

        private (int number, Color color) calculatePips(int opinion) {
            var blocks = 0;
            var col = Color.Black;
            if (opinion < MindConstants.OPINION_HATE) {
                blocks = MindConstants.OPINION_HATE - opinion;
                col = Pips.red;
            } else if (opinion <= MindConstants.OPINION_WARY) {
                blocks = MindConstants.OPINION_WARY - opinion;
                col = Pips.orange;
            } else if (opinion <= MindConstants.OPINION_ALLY) {
                blocks = opinion + MindConstants.OPINION_ALLY;
                col = Pips.yellow;
            } else if (opinion <= MindConstants.OPINION_FRIEND) {
                blocks = opinion - MindConstants.OPINION_ALLY;
                col = Pips.blue;
            } else {
                blocks = opinion - MindConstants.OPINION_FRIEND;
                col = Pips.green;
            }
            return (1 + (blocks / 40), col);
        }
    }
}