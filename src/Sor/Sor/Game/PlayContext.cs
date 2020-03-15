using System.Collections.Generic;
using System.Linq;
using Glint;
using Glint.Util;
using LunchLib.Calc;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI;
using Sor.AI.Cogs;
using Sor.Components.Input;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Game.Map;
using Sor.Game.Map.Gen;
using Sor.Scenes;

namespace Sor.Game {
    public class PlayContext {
        public Wing playerWing;

        public IEnumerable<Wing> wings =>
            scene.FindEntitiesWithTag(Constants.Tags.WING).Select(x => x.GetComponent<Wing>());

        public List<Wing> createdWings = new List<Wing>();
        public List<Thing> createdThings = new List<Thing>();
        public Entity mapNt;
        public int mapgenSeed = 0;
        public bool rehydrated = false;

        public PlayScene scene;
        public MapLoader mapLoader;

        public void addThing(Thing thing) {
            createdThings.Add(thing);
        }

        public Wing createPlayer(Vector2 pos) {
            var playerNt = new Entity("player").SetTag(Constants.Tags.WING);
            var playerSoul = new AvianSoul();
            playerSoul.ply.generateNeutral();
            playerWing = playerNt.AddComponent(new Wing(new Mind(playerSoul, false)));
            playerWing.body.pos = pos;
            playerNt.AddComponent<PlayerInputController>();
            return playerWing;
        }

        public Wing createWing(string name, Vector2 pos, BirdPersonality ply) {
            var duckNt = new Entity(name).SetTag(Constants.Tags.WING);
            var duck = duckNt.AddComponent(new Wing(new Mind(new AvianSoul {ply = ply}, true)));
            duck.body.pos = pos;
            duckNt.AddComponent<LogicInputController>();
            createdWings.Add(duck);
            return duck;
        }

        private void loadMap() {
            // TODO: figure out whether we're creating a new map or restoring (does it matter if we seed the rng)
            // var mapAsset = Core.Content.LoadTiledMap("Data/maps/test3.tmx");
            var mapAsset = Core.Content.LoadTiledMap("Data/maps/base.tmx");
            var genMapSize = NGame.context.config.generatedMapSize;
            // var genMapSize = 16;
            if (mapgenSeed == 0) {
                mapgenSeed = Random.RNG.Next(int.MinValue, int.MaxValue);
            }

            var gen = new MapGenerator(genMapSize, genMapSize, mapgenSeed);
            gen.generate();
            gen.analyze();
            gen.copyToTilemap(mapAsset, createObjects: !rehydrated);
            // log generated map
            Glint.Global.log.writeLine(
                $"generated map of size {genMapSize}, with {gen.roomRects.Count} rects:\n{gen.dumpGrid()}",
                GlintLogger.LogLevel.Trace);
            // TODO: ensure that the loaded map matches the saved map
            mapNt = new Entity("map");
            var mapRenderer = mapNt.AddComponent(new TiledMapRenderer(mapAsset, null, false));
            mapRenderer.SetLayersToRender(MapLoader.LAYER_STRUCTURE, MapLoader.LAYER_FEATURES);
            mapLoader = new MapLoader(this, mapNt);

            // load map
            mapLoader.load(mapAsset, createObjects: !rehydrated);
        }

        public void load() {
            loadMap();
            if (!rehydrated) {
                // fresh load
                spawnBirds();
            }
        }

        private void spawnBirds() {
            // now, spawn a bunch of birds across the rooms
            int spawnedBirds = 0;
            var birdSpawnRng = new Rng(Random.NextInt(int.MaxValue));
            var birdClassDist = new DiscreteProbabilityDistribution<Wing.WingClass>(birdSpawnRng, new[] {
                (0.5f, Wing.WingClass.Wing),
                (0.3f, Wing.WingClass.Beak),
                (0.2f, Wing.WingClass.Predator)
            });
            foreach (var room in mapLoader.mapRepr.roomGraph.rooms) {
                var roomBirdProb = 0.2f;
                if (Random.Chance(roomBirdProb)) {
                    spawnedBirds++;
                    var spawnPos = mapLoader.mapRepr.tmxMap.TileToWorldPosition(room.center.ToVector2());
                    var spawnPly = new BirdPersonality();
                    spawnPly.generateRandom();
                    var bord = createWing($"b_{spawnedBirds}", spawnPos, spawnPly);

                    bord.changeClass(birdClassDist.next());
                }
            }

            Global.log.writeLine($"spawned {spawnedBirds} birds across the map", GlintLogger.LogLevel.Trace);
        }
    }
}