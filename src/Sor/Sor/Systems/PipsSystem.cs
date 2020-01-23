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
            if (opinion < -300) {
                blocks = -300 - opinion;
                col = Pips.red;
            } else if (opinion <= -100) {
                blocks = -100 - opinion;
                col = Pips.orange;
            } else if (opinion <= 100) {
                blocks = opinion + 100;
                col = Pips.yellow;
            } else if (opinion <= 300) {
                blocks = opinion - 100;
                col = Pips.blue;
            } else {
                blocks = opinion - 300;
                col = Pips.green;
            }
            return (blocks / 4, col);
        }
    }
}