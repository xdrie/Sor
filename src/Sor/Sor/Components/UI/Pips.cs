using Glint.Sprites;
using Nez;

namespace Sor.Components.UI {
    public class Pips : GAnimatedSprite {
        public Pips() : base(Core.Content.LoadTexture("Data/sprites/pips.png"), 16, 16) { }

        public override void Initialize() {
            base.Initialize();
            
            animator.AddAnimation("one", new[] {sprites[0]});
            animator.AddAnimation("two", new[] {sprites[1]});
            animator.AddAnimation("three", new[] {sprites[2]});
            animator.AddAnimation("four", new[] {sprites[3]});
            animator.AddAnimation("five", new[] {sprites[4]});
        }
    }
}