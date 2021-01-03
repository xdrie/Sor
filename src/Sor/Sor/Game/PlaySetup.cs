using System.Collections.Generic;
using Glint;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Sor.AI;
using Sor.AI.Cogs;
using Sor.Components.Input;
using Sor.Components.Items;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Game.Map;
using Sor.Game.Map.Gen;

namespace Sor.Game {
    /// <summary>
    /// utility class for preparing data for Play Scene and State
    /// </summary>
    public class PlaySetup {
        public List<Wing> wings = new List<Wing>();
        public List<Thing> things = new List<Thing>();
        public Entity mapNt;

        public bool rehydrated = false;

        /// <summary>
        /// the target state to be populated
        /// </summary>
        public PlayState state;

        /// <summary>
        /// create a setup for a fresh state
        /// </summary>
        public PlaySetup() : this(new PlayState()) { }

        public PlaySetup(PlayState state) {
            this.state = state;
        }

        #region Wing and Thing spawning

        /// <summary>
        /// create a wing and add it to the list
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <param name="mind"></param>
        /// <returns></returns>
        public Wing createWing(string name, Vector2 pos, DuckMind mind) {
            var wingNt = new Entity(name).SetTag(Constants.Tags.WING);
            var wing = wingNt.AddComponent(new Wing(mind));
            wing.body.pos = pos;
            wings.Add(wing);
            return wing;
        }

        /// <summary>
        /// instantiate the player wing and add components
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Wing createPlayer(Vector2 pos) {
            var playerSoul = new AvianSoul();
            playerSoul.ply = BirdPersonality.makeNeutral();
            var mind = new DuckMind(playerSoul, false);
            var player = createWing(Constants.Game.PLAYER_NAME, pos, mind);
            player.AddComponent(new PlayerInputController(0));
            state.player = player; // save player
            return player;
        }

        /// <summary>
        /// create an NPC (autonomous/ai-controlled) wing
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <param name="ply"></param>
        /// <returns></returns>
        public Wing createNpcWing(string name, Vector2 pos, BirdPersonality ply) {
            var mind = new DuckMind(new AvianSoul {ply = ply}, true);
            var wing = createWing(name, pos, mind);
            wing.AddComponent<LogicInputController>();
            return wing;
        }

        public void addThing(Thing thing) {
            if (thing == null) {
                Global.log.err("attempted to add null thing to things list.");
            }
            things.Add(thing);
        }

        #endregion

        #region Loading and Scene setup

        public void load() {
            loadMap();
            if (!rehydrated) {
                // fresh load
                createEcosystem();
            }
        }

        private void loadMap() {
            // TODO: figure out whether we're creating a new map or restoring (does it matter if we seed the rng)
            var mapAsset = default(TmxMap);
            if (NGame.context.config.generateMap) {
                mapAsset = Core.Content.LoadTiledMap("Data/maps/base.tmx");
                var mapSize = NGame.context.config.generatedMapSize;

                if (state.mapgenSeed == 0) {
                    // choose a new mapgen seed
                    state.mapgenSeed = Random.RNG.Next(int.MinValue, int.MaxValue);
                }

                var gen = new MapGenerator(mapSize, mapSize, state.mapgenSeed);
                gen.generate();
                gen.analyze();
                gen.copyToTilemap(mapAsset, createObjects: !rehydrated);
                // log generated map
                Global.log.trace(
                    $"generated map of size {mapSize}, with {gen.roomRects.Count} rects:\n{gen.dumpGrid()}");
            }
            else {
                mapAsset = Core.Content.LoadTiledMap("Data/maps/test4.tmx");
            }

            // TODO: ensure that the loaded map matches the saved map
            mapNt = new Entity("map");
            var mapRenderer = mapNt.AddComponent(new TiledMapRenderer(mapAsset, null, false));
            mapRenderer.SetLayersToRender(MapLoader.LAYER_STRUCTURE, MapLoader.LAYER_FEATURES,
                MapLoader.LAYER_BACKDROP);
            var mapLoader = new MapLoader(this, mapNt);

            // load map
            mapLoader.load(mapAsset, createObjects: !rehydrated);

            // save map
            state.map = mapLoader.mapRepr;
        }

        /// <summary>
        /// create and set up the ecosystem (living members) of the scene
        /// </summary>
        private void createEcosystem() {
            var spawn = new Vector2(200, 200);
            if (NGame.config.generateMap) {
                var mapBounds = state.map.tmxMap.TileToWorldPosition(new Vector2(state.map.tmxMap.Width,
                    state.map.tmxMap.Height));
                spawn = new Vector2(Random.NextFloat() * mapBounds.X, Random.NextFloat() * mapBounds.Y);
            }

            state.player = createPlayer(spawn);
            state.player.Entity.AddComponent(new Shooter()); // arm the player

            // a friendly bird
            var frend = createNpcWing("frend", spawn + new Vector2(-140, 20),
                new BirdPersonality {A = -0.8f, S = 0.7f});
            frend.AddComponent(new Shooter()); // friend is armed

            // // unfriendly bird
            // var enmy = createWing("enmy", new Vector2(120, 80),
            //     new BirdPersonality {A = 0.8f, S = -0.7f});
            // enmy.AddComponent(new Shooter()); // enmy is armed

            // if spawning is enabled, create additional birds
            if (NGame.context.config.spawnBirds) {
                var unoPly = new BirdPersonality();
                unoPly = BirdPersonality.makeNeutral();
                var uno = createNpcWing("uno", spawn + new Vector2(-140, 920), unoPly);
                uno.changeClass(Wing.WingClass.Predator);

                // a second friendly bird
                var fren2 = createNpcWing("yii", spawn + new Vector2(400, -80),
                    new BirdPersonality {A = -0.5f, S = 0.4f});
                // a somewhat anxious bird
                var anxious1 = createNpcWing("ada", spawn + new Vector2(640, 920),
                    new BirdPersonality {A = 0.6f, S = -0.2f});

                // generate random birds spread across the map
                var gen = new BirdGenerator(this);
                gen.spawnBirds();
            }
        }

        #endregion
    }
}