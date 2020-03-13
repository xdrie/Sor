using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Sor.Components.Input;

namespace Sor.Components.UI {
    public class MenuButtonList : Component, IUpdatable {
        private Vector2 offset;
        private List<Item> items;
        private readonly List<Sprite> buttonSprites;
        private const string NOT_SELECTED = "not";
        private const string YES_SELECTED = "sel";

        public int selectedItem = 0;
        public bool active = true;

        public MenuButtonList(List<Item> items, List<Sprite> buttonSprites, Vector2 offset) {
            this.items = items;
            this.buttonSprites = buttonSprites;
            this.offset = offset;
        }

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            
            // create button animators from button texture
            var currentOffset = offset;
            foreach (var item in items) {
                // add button frame
                item.buttonAnim = new SpriteAnimator(buttonSprites[0]);
                item.buttonAnim.AddAnimation(NOT_SELECTED, new[] { buttonSprites[0] });
                item.buttonAnim.AddAnimation(YES_SELECTED, new[] { buttonSprites[1] });
                item.buttonAnim.SetLocalOffset(currentOffset);
                Entity.AddComponent(item.buttonAnim);
                // also add button text
                item.texRen = new SpriteRenderer(item.textSpr);
                item.texRen.SetLocalOffset(currentOffset); // center of button frame matches text pos
                Entity.AddComponent(item.texRen);
                currentOffset += new Vector2(0, item.buttonAnim.Sprite.Texture2D.Height);
            }
        }

        public class Item {
            public Sprite textSpr;
            public SpriteRenderer texRen;
            public SpriteAnimator buttonAnim;
            public Action onSelected;

            public Item(Sprite textSpr, Action onSelected) {
                this.textSpr = textSpr;
                this.onSelected = onSelected;
            }
        }

        public void Update() {
            if (!active) return;
            
            // check input and update the selected sprite
            foreach (var item in items) { // deselect all
                item.buttonAnim.Play(NOT_SELECTED);
                // set text color
                item.texRen.Color = NGame.context.assets.paletteBrown;
            }
            // show on the selected one
            items[selectedItem].buttonAnim.Play(YES_SELECTED);
            items[selectedItem].texRen.Color = NGame.context.assets.paletteWhite;
            
            // check input to update selection
            var controller = Entity.GetComponent<MenuInputController>();

            if (controller.interact.IsPressed) {
                items[selectedItem].onSelected?.Invoke();
            }

            var ds = 0;
            if (controller.navDown.IsPressed) {
                ds = 1;
            } else if (controller.navUp.IsPressed) {
                ds = -1;
            }

            selectedItem = (items.Count + selectedItem + ds) % items.Count;
        }

        public void applyToRenderers(Action<SpriteRenderer> renAction) {
            foreach (var item in items) {
                renAction(item.buttonAnim);
                renAction(item.texRen);
            }
        }
    }
}