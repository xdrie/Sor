using Glint.Sprites;
using Nez;

namespace Sor.Components.Things {
    public class Tree : GAnimatedSprite {
        public int stage = 1;
        
        public Tree() : base(Core.Content.LoadTexture("Data/sprites/tree.png"), 64, 64) {
            animator.AddAnimation("1", new[] {sprites[0]});
            animator.AddAnimation("2", new[] {sprites[1]});
            animator.AddAnimation("3", new[] {sprites[2]});
            animator.AddAnimation("4", new[] {sprites[3]});
            animator.AddAnimation("5", new[] {sprites[4]});
            animator.AddAnimation("6", new[] {sprites[5]});
            animator.AddAnimation("7", new[] {sprites[6]});
            animator.AddAnimation("8", new[] {sprites[7]});
            animator.AddAnimation("9", new[] {sprites[8]});
            animator.AddAnimation("10", new[] {sprites[9]});
        }

        public override void Initialize() {
            base.Initialize();
            
            animator.Play(stage.ToString());
        }
    }
}