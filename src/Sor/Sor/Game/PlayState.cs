using System.Collections.Generic;
using System.Linq;
using Glint;
using Glint.Util;
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
using Sor.Scenes;

namespace Sor.Game {
    /// <summary>
    /// Represents all the state information of an in-progress game
    /// </summary>
    public class PlayState {
        public Wing player;

        public int mapgenSeed = 0;

        public PlayScene scene;
        public MapRepr map;

        public IEnumerable<Wing> findAllWings() =>
            scene.FindEntitiesWithTag(Constants.Tags.WING).Select(x => x.GetComponent<Wing>());
    }
}