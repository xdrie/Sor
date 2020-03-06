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
                var buttonTextRen = new SpriteRenderer(item.textSpr);
                buttonTextRen.SetLocalOffset(currentOffset); // center of button frame matches text pos
                Entity.AddComponent(buttonTextRen);
                currentOffset += new Vector2(0, item.buttonAnim.Sprite.Texture2D.Height);
            }
        }

        public class Item {
            public Sprite textSpr;
            public SpriteAnimator buttonAnim;
            public Action selected;

            public Item(Sprite textSpr, Action selected) {
                this.textSpr = textSpr;
                this.selected = selected;
            }
        }

        public void Update() {
            // check input and update the selected sprite
            foreach (var item in items) { // deselect all
                item.buttonAnim.Play(NOT_SELECTED);
            }
            // show on the selected one
            items[selectedItem].buttonAnim.Play(YES_SELECTED);
            
            // check input to update selection
            var controller = Entity.GetComponent<InputController>();
            var navInput = controller.moveDirectionInput;
            var pressed = controller.interactInput.IsPressed;
        }
    }
}