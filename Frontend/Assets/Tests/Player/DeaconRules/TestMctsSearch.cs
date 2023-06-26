using System;
using NUnit.Framework;
using Code.Player.DeaconRules;
using Code.Player.MCTS;
using UnityEngine;
using UnityEngine.Scripting;

namespace Tests.Player.DeaconRules
{
    using Player = Code.Player.DeaconRules.Player;

    public class TestMctsSearch
    {
        [Test, Timeout(1000)]
        public void MctsSearchHalts()
        {
            var searcher = new Searcher<Player, GameState, GameAction>(
                GameState.INITIAL_STATE,
                new SearchParameters { ExplorationFactor = 1.0 / Math.Sqrt(2.0), Iterations = 100 }
            );

            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
            searcher.Search();
            GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;

            Debug.Log(GameState._GAA_CACHE.Count);
            Debug.Log(GameState.cachehits);
        }
    }
}
