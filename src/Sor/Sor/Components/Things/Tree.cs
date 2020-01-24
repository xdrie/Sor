using System.Collections.Generic;
using Glint.Sprites;
using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Components.Things {
    public class Tree : Thing, IUpdatable {
        public int stage = 1;
        public int fruits = 0;
        public int harvest = 0;
        public List<Capsule> childFruits = new List<Capsule>();
        public int maxFruits = 0;
        public float ripeningTime = 0.4f;
        public float developmentSpeed = 2f; // development speed is a ratio
        public float growthTimer = 0;
        public float fruitTimer = 0f;
        public float fruitSpawnRange = 10f;
        public float childRange = 40f;
        public float fruitValue = 800f;

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
            
            updateStage();
            UpdateInterval = 10;
        }

        public void updateStage() {
            animator.Play(stage.ToString());
            growthTimer = Time.TotalTime + 60f * (1f / developmentSpeed) * stage; // time until next growth
            switch (stage) {
                case 7:
                    maxFruits = 1;
                    break;
                case 8:
                    maxFruits = 2;
                    break;
                case 9:
                    maxFruits = 3;
                    break;
                case 10:
                    maxFruits = 5;
                    break;
                default:
                    maxFruits = 0;
                    break;
            }
        }

        public void Update() {
            var fruitsPerSec = developmentSpeed;
            var growFruit = Nez.Random.Chance(fruitsPerSec * Time.DeltaTime * UpdateInterval);
            if (fruits < maxFruits && Time.TotalTime > fruitTimer && growFruit) {
                // spawn a fruit
                var fruitOffset = Random.Range(new Vector2(-fruitSpawnRange), new Vector2(fruitSpawnRange));
                var capNt = Entity.Scene.CreateEntity(null, Entity.Position + fruitOffset)
                    .SetTag(Constants.ENTITY_THING);
                var fruit = capNt.AddComponent<Capsule>();
                fruit.firstAvailableAt = Time.TotalTime + ripeningTime;
                fruit.creator = this;
                fruit.energy = Nez.Random.Range(fruitValue * 0.6f, fruitValue * 1.2f);
                fruit.body.velocity = Vector2.Zero;
                childFruits.Add(fruit);
                fruits++;
                
                fruitTimer = Time.TotalTime + developmentSpeed * 10f;
            }

            // update existing fruits
            var growthPoints = 0;
            if (fruits > 0) {
                var rmFruits = new HashSet<Capsule>();
                foreach (var child in childFruits) {
                    var toChild = Entity.Position - child.Entity?.Position;
                    if (child.acquired || !toChild.HasValue || toChild.Value.LengthSquared() > childRange * childRange) {
                        fruits--;
                        harvest++;
                        growthPoints++;
                        rmFruits.Add(child);
                    }
                }

                childFruits.RemoveAll(x => rmFruits.Contains(x));
            }
            
            // update general tree growth
            growthTimer -= developmentSpeed * growthPoints * 10f;
            if (Time.TotalTime > growthTimer) {
                stage++; // upgrade stage
                updateStage();
            }
        }
    }
}