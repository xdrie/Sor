using System.Linq;
using Glint;
using Glint.Util;
using LunchLib.Calc;
using Nez;
using Sor.AI.Cogs;
using Sor.Components.Items;
using Sor.Components.Units;
using Sor.Util;

namespace Sor.Game {
    public class BirdGenerator {
        private readonly PlayContext playContext;

        public enum BirdEquipment {
            Bare,
            Equip1,
            Equip2,
            Equip3,
        }

        public BirdGenerator(PlayContext playContext) {
            this.playContext = playContext;
        }

        public void spawnBirds() {
            // now, spawn a bunch of birds across the rooms
            int spawnedBirds = 0;
            var birdSpawnRng = new Rng(Random.NextInt(int.MaxValue));
            var birdClassDist = new DiscreteProbabilityDistribution<Wing.WingClass>(birdSpawnRng, new[] {
                (0.5f, Wing.WingClass.Wing),
                (0.3f, Wing.WingClass.Beak),
                (0.2f, Wing.WingClass.Predator)
            });
            var birdEquipmentDist = new DiscreteProbabilityDistribution<BirdEquipment>(birdSpawnRng, new [] {
                (0.7f, BirdEquipment.Bare),
                (0.3f, BirdEquipment.Equip1)
            });
            foreach (var room in playContext.mapLoader.mapRepr.roomGraph.rooms) {
                var roomBirdProb = 0.2f;
                if (Random.Chance(roomBirdProb)) {
                    spawnedBirds++;
                    
                    // generate spawn attributes
                    var spawnPos = playContext.mapLoader.mapRepr.tmxMap.TileToWorldPosition(room.center.ToVector2());
                    var spawnPly = new BirdPersonality();
                    spawnPly.generateRandom();
                    
                    var bordClass = birdClassDist.next();
                    var className = bordClass.ToString().ToLower().First();
                    var nick = NameGenerator.next().ToLowerInvariant();
                    
                    // create the wing
                    var bord = playContext.createWing($"{nick} {className}", spawnPos, spawnPly);
                    bord.changeClass(bordClass);
                    
                    // equip the wing
                    var loadout = birdEquipmentDist.next();
                    switch (loadout) {
                        case BirdEquipment.Equip1:
                            bord.AddComponent(new Shooter());
                            break;
                    }
                }
            }

            Global.log.writeLine($"spawned {spawnedBirds} birds across the map", GlintLogger.LogLevel.Trace);
        }
    }
}