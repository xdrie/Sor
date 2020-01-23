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
                    wing.pips.setPips(pipCount, pipColor);
                } else {
                    wing.pips.disable();
                }
            }
        }

        private (int number, Color color) calculatePips(int opinion) {
            var col = Color.White;
            if (opinion > 0) {
                col = Pips.green;
            } else {
                col = Pips.red;
            }
            return (1, col);
        }
    }
}