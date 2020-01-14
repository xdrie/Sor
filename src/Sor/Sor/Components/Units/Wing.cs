using Glint.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Sor.Components.UI;

namespace Sor.Components.Units {
    public class Wing : GAnimatedSprite {
        private WingBody body;
        private BoxCollider hitbox;
        
        public Wing() : base(Core.Content.LoadTexture("Data/sprites/ship.png"), 64, 64) { }

        public override void Initialize() {
            base.Initialize();

            animator.AddAnimation("ship", new[] {sprites[0]});
            animator.AddAnimation("ship2block", new[] {sprites[1], sprites[2], sprites[3], sprites[4]});
            animator.AddAnimation("block", new[] {sprites[5]});
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            
            body = Entity.AddComponent(new WingBody {mass = 10f});
            hitbox = Entity.AddComponent(new BoxCollider(-4, -6, 8, 12) {Tag = Constants.TAG_SHIP_COLLIDER});
            // add pips
            var pips = Entity.AddComponent<Pips>();
            pips.spriteRenderer.LocalOffset = new Vector2(0, 14);
            pips.animator.Play("three");
        }
    }
}