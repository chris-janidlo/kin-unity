using System;
using NUnit.Framework;
using Code.Player.DeaconRules;
using Code.Player.MCTS;

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
                new SearchParameters { ExplorationFactor = 1.0 / Math.Sqrt(2.0), Iterations = 1000 }
            );

            searcher.Search();
        }
    }
}
