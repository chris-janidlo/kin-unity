using System;
using System.Collections.Generic;
using Code.Player.MCTS;
using NUnit.Framework;
using Unity.Collections;

namespace Tests.Player.MCTS
{
    public class TestSearcher
    {
        private static Searcher<TestPlayer, TestState, TestAction> BasicSearcher()
        {
            var parameters = new SearchParameters
            {
                ExplorationFactor = TestMctsConstants.EXPLORATION_FACTOR,
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

        [Test, Timeout(1000)]
        public void SearchReturnsLegalMove()
        {
            var searcher = BasicSearcher();
            TestAction move = searcher.Search();

            var rootState = new TestState(0);

            var actionBuffer = new NativeList<TestAction>(
                rootState.ActionArrayMaxSize,
                Allocator.Temp
            );
            using (actionBuffer)
            {
                rootState.GetAvailableActions(ref actionBuffer);

                var found = false;

                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (TestAction action in actionBuffer)
                    if (action.Addend == move.Addend)
                    {
                        found = true;
                        break;
                    }

                Assert.True(
                    found,
                    $"unable to find {move.Addend} in {rootState.Value}'s legal moves"
                );
            }
        }
    }
}
