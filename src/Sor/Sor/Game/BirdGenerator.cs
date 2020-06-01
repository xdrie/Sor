using System.Linq;
using Glint;
using Glint.Util;
using Ducia.Calc;
using Nez;
using Sor.AI.Cogs;
using Sor.Components.Items;
using Sor.Components.Units;
using Sor.Util;

namespace Sor.Game {
    public class BirdGenerator {
        private readonly PlayState state;

        public enum BirdEquipment {
            Bare,
            Equip1,
            Equip2,
            Equip3,
        }

        public BirdGenerator(PlayState state) {
            this.state = state;
        }

        /// <summary>
        /// create a whole bunch of birds across the map rooms
        /// </summary>
        public void spawnBirds() {
            int spawnedBirds = 0;
            var birdSpawnRng = new Rng(Random.NextInt(int.MaxValue));
            var birdClassDist = new DiscreteProbabilityDistribution<Wing.WingClass>(birdSpawnRng, new[] {
                (0.5f, Wing.WingClass.Wing),
                (0.3f, Wing.WingClass.Beak),
                (0.2f, Wing.WingClass.Predator)
            });
            var birdEquipmentDist = new DiscreteProbabilityDistribution<BirdEquipment>(birdSpawnRng, new[] {
                (0.7f, BirdEquipment.Bare),
                (0.3f, BirdEquipment.Equip1)
            });
            foreach (var room in state.mapLoader.mapRepr.roomGraph.rooms) {
                var roomBirdProb = 0.2f;
                if (Random.Chance(roomBirdProb)) {
                    spawnedBirds++;

                    // generate spawn attributes
                    var spawnPos = state.mapLoader.mapRepr.tmxMap.TileToWorldPosition(room.center.ToVector2());
                    var spawnPly = new BirdPersonality();
                    spawnPly = BirdPersonality.makeRandom();

                    var bordClass = birdClassDist.next();
                    var className = bordClass.ToString().ToLower().First();
                    var nick = NameGenerator.next().ToLowerInvariant();

                    // create the wing
                    var bord = state.createNpcWing($"{nick} {className}", spawnPos, spawnPly);
                    bord.changeClass(bordClass, true);

                    // equip the wing
                    var loadout = birdEquipmentDist.next();
                    switch (loadout) {
                        case BirdEquipment.Equip1:
                            bord.AddComponent(new Shooter());
                            break;
                    }
                }
            }

            Global.log.trace($"spawned {spawnedBirds} birds across the map");
        }
    }
}