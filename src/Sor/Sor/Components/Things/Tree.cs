using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Random = Nez.Random;

namespace Sor.Components.Things {
    public class Tree : Thing, IUpdatable {
        public int stage = 1;
        public int harvest = 0;

        public int fruits = 0;
        public List<Capsule> childFruits = new List<Capsule>();
        public int maxFruits = 0;
        public float growthTimer = 0;
        public float fruitTimer = 0f;
        
        public const float ripeningTime = 0.4f;
        public const float developmentSpeed = 2f; // development speed is a ratio, more means faster
        public const float fruitSpawnRange = 10f;
        public const float childRange = 40f;
        public const float fruitBaseValue = 800f;
        
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
            // check if there is an animation for the next stage
            var stageAnim = stage.ToString();
            if (animator.Animations.ContainsKey(stageAnim)) {
                animator.Play(stageAnim);
            }
            growthTimer = Time.TotalTime + 60f * (1f / developmentSpeed) * stage; // time until next growth
            maxFruits = stage switch {
                6 => 1,
                7 => 3,
                8 => 6,
                9 => 9,
                10 => 14,
                _ => 0
            };
        }

        public void Update() {
            var fruitsPerSec = developmentSpeed;
            var growFruit = Random.Chance(fruitsPerSec * Time.DeltaTime * UpdateInterval);
            if (fruits < maxFruits && Time.TotalTime > fruitTimer && growFruit) {
                // spawn a fruit
                var fruitOffset = Random.Range(new Vector2(-fruitSpawnRange), new Vector2(fruitSpawnRange));
                var capNt = Entity.Scene.CreateEntity(null, Entity.Position + fruitOffset)
                    .SetTag(Constants.Tags.THING);
                var fruit = capNt.AddComponent<Capsule>();
                fruit.firstAvailableAt = Time.TotalTime + ripeningTime;
                fruit.creator = this;
                fruit.energy = Random.Range(fruitBaseValue * 0.6f, fruitBaseValue * 2.2f);
                fruit.body.velocity = Vector2.Zero;
                childFruits.Add(fruit);
                fruits++;
                
                fruitTimer = Time.TotalTime + Random.Range(1.4f, 3.4f) * developmentSpeed;
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
                if (stage > 10) stage = 10; // clamp to max stage
                updateStage();
            }
        }
    }
}