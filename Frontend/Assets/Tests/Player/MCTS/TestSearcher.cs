using System;
using System.Collections.Generic;
using Code.Player.MCTS;
using NUnit.Framework;
using Unity.Collections;

namespace Tests.Player.MCTS
{
    using ConcreteNode = SearchTreeNode<TestPlayer, TestState, TestAction>;
    using ConcreteChild = ChildSearchTreeNode<TestPlayer, TestState, TestAction>;

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

        [Test, Timeout(1000)]
        public void SearchForgetsUnnecessaryBranches()
        {
            var searcher = BasicSearcher();
            var oldRoot = searcher.Tree;

            searcher.Search();

            var newRoot = searcher.Tree;

            Assert.That(newRoot, Is.Not.SameAs(oldRoot));
            Assert.That(newRoot.Children, Is.Not.Empty);
            Assert.That(oldRoot.Children, Has.No.Member(newRoot));
            if (newRoot is ConcreteChild child)
                Assert.That(child.Parent, Is.Null);
            else
                Assert.Fail("expected detached child");
        }

        [Test]
        public void ApplyActionCreatesNewTree()
        {
            var searcher = BasicSearcher();

            var root = searcher.Tree;
            Helper.RandomChild(root);
            Helper.RandomChild(root);
            Helper.RandomChild(root);

            var actionToApply = new TestAction(int.MaxValue);

            searcher.ApplyAction(actionToApply);

            var newRoot = searcher.Tree;
            Assert.That(newRoot, Is.Not.EqualTo(root));
            Assert.That(newRoot.Children, Is.Empty);
        }

        [Test]
        public void ApplyActionDetachesExistingBranch()
        {
            var searcher = BasicSearcher();

            var root = searcher.Tree;
            var child = Helper.RandomChild(root);
            var child2 = Helper.RandomChild(root);
            if (child2.IncomingAction.Addend == child.IncomingAction.Addend)
                child2.IncomingAction = new TestAction(child2.IncomingAction.Addend * 2);

            searcher.ApplyAction(child.IncomingAction);

            Assert.That(searcher.Tree, Is.EqualTo(child));
            Assert.That(child.Parent, Is.Null);
            Assert.That(root.Children, Has.No.Member(child));
        }
    }
}
