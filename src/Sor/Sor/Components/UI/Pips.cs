using Glint.Sprites;
using Nez;

namespace Sor.Components.UI {
    public class Pips : GAnimatedSprite, IUpdatable {
        public Pips() : base(Core.Content.LoadTexture("Data/sprites/pips.png"), 16, 16) { }

        public override void Initialize() {
            base.Initialize();
            
            animator.AddAnimation("1", new[] {sprites[0]});
            animator.AddAnimation("2", new[] {sprites[1]});
            animator.AddAnimation("3", new[] {sprites[2]});
            animator.AddAnimation("4", new[] {sprites[3]});
            animator.AddAnimation("5", new[] {sprites[4]});
        }
    }
}