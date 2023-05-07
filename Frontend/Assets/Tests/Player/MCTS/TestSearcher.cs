using System;
using Code.Player.MCTS;
using NUnit.Framework;

namespace Tests.Player.MCTS
{
    public class TestSearcher
    {
        private static Searcher<TestPlayer, TestState, TestAction> BasicSearcher()
        {
            var parameters = new SearchParameters
            {
                ExplorationFactor = 1.0 / Math.Sqrt(2.0),
                Iterations = 20
            };

            var startState = new TestState(0);

            return new Searcher<TestPlayer, TestState, TestAction>(startState, parameters);
        }

        [Test, Timeout(1000)]
        public void SearchTerminates()
        {
            var searcher = BasicSearcher();
            searcher.Search();
        }
    }
}
