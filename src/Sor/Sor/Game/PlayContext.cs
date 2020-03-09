using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI;
using Sor.AI.Cogs;
using Sor.Components.Input;
using Sor.Components.Things;
using Sor.Components.Units;
using Sor.Scenes;

namespace Sor.Game {
    public class PlayContext {
        public Wing playerWing;

        public IEnumerable<Wing> wings =>
            scene.FindEntitiesWithTag(Constants.Tags.ENTITY_WING).Select(x => x.GetComponent<Wing>());

        public List<Wing> createdWings = new List<Wing>();
        public List<Thing> createdThings = new List<Thing>();
        public bool rehydrated = false;

        public PlayScene scene;

        public void addThing(Thing thing) {
            createdThings.Add(thing);
        }

        public void createPlayer(Vector2 pos) {
            var playerNt = new Entity("player").SetTag(Constants.Tags.ENTITY_WING);
            var playerSoul = new AvianSoul();
            playerSoul.ply.generateNeutral();
            playerWing = playerNt.AddComponent(new Wing(new Mind(playerSoul, false)));
            playerWing.body.pos = pos;
            playerNt.AddComponent<PlayerInputController>();
        }

        public Wing createWing(string name, Vector2 pos, BirdPersonality ply) {
            var duckNt = new Entity(name).SetTag(Constants.Tags.ENTITY_WING);
            var duck = duckNt.AddComponent(new Wing(new Mind(new AvianSoul {ply = ply}, true)));
            duck.body.pos = pos;
            duckNt.AddComponent<LogicInputController>();
            createdWings.Add(duck);
            return duck;
        }
    }
}