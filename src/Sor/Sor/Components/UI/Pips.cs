using Glint.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace Sor.Components.UI {
    public class Pips : GAnimatedSprite {
        public Pips() : base(Core.Content.LoadTexture("Data/sprites/pips.png"), 16, 16) { }

        public static Color red => Color.Red;
        public static Color orange => Color.Orange;
        public static Color yellow => Color.Yellow;
        public static Color blue => Color.Blue;
        public static Color green => Core.Services.GetService<GameContext>().assets.success;

        public SpriteAnimator colAnimator;

        public override void Initialize() {
            base.Initialize();
            
            animator.AddAnimation("1", new[] {sprites[0]});
            animator.AddAnimation("2", new[] {sprites[1]});
            animator.AddAnimation("3", new[] {sprites[2]});
            animator.AddAnimation("4", new[] {sprites[3]});
            animator.AddAnimation("5", new[] {sprites[4]});
            
            var colSprite = Sprite.SpritesFromAtlas(new Sprite(Core.Content.LoadTexture("Data/sprites/pips_col.png")),
                16, 16);
            colAnimator = new SpriteAnimator(colSprite[0]);
            colAnimator.AddAnimation("1", new[] {colSprite[0]});
            colAnimator.AddAnimation("2", new[] {colSprite[1]});
            colAnimator.AddAnimation("3", new[] {colSprite[2]});
            colAnimator.AddAnimation("4", new[] {colSprite[3]});
            colAnimator.AddAnimation("5", new[] {colSprite[4]});
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            
            Entity.AddComponent(colAnimator);
            colAnimator.LocalOffset = animator.LocalOffset;
            colAnimator.Play(animator.CurrentAnimationName);
        }

        public void setPips(int number, Color color) {
            var animName = number.ToString();
            animator.Play(animName);
            colAnimator.Color = color;
            colAnimator.Play(animName);
        }

        public void enable() {
            Enabled = true;
            spriteRenderer.Enabled = true;
        }

        public void disable() {
            Enabled = false;
            spriteRenderer.Enabled = false;
        }
    }
}