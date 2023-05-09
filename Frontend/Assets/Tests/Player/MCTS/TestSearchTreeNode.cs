using System.Collections.Generic;
using Code.Player.MCTS;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Player.MCTS
{
    using ConcreteNode = SearchTreeNode<TestPlayer, TestState, TestAction>;
    using ConcreteChild = ChildSearchTreeNode<TestPlayer, TestState, TestAction>;

    internal class Helper
    {
        internal static ConcreteNode RandomRoot()
        {
            var state = new TestState(Random.Range(0, 10));

            return new ConcreteNode(state)
            {
                Score = Random.Range(-14.0f, 14.0f),
                Visits = Random.Range(1, 100)
            };
        }

        internal static ConcreteChild RandomChild(ConcreteNode parent)
        {
            var state = new TestState(Random.Range(0, 10));
            var action = new TestAction(state.Value - parent.GameState.Value);

            return new ConcreteChild(state, parent, action)
            {
                Score = Random.Range(-14.0f, 14.0f),
                Visits = Random.Range(1, 100)
            };
        }
    }

    public class TestSearchTreeNode
    {
        [Test]
        public void ExpandAddsNewNodeAsChildOfOriginal()
        {
            var parent = Helper.RandomRoot();
            var expanded = parent.Expand();

            Assert.That(parent.Children, Has.Member(expanded));
            Assert.That(expanded.Parent, Is.EqualTo(parent));
        }

        [Test]
        public void ExpandFindsMoveToExpandOn()
        {
            var parent = Helper.RandomRoot();
            var node = Helper.RandomChild(parent);

            var expanded = node.Expand();

            Assert.That(expanded, Is.Not.EqualTo(node));
        }

        [Test]
        public void ExpandConsumesUnvisitedActions()
        {
            var node = Helper.RandomRoot();

            var originalUnvisitedActions = new HashSet<TestAction>(node.UnvisitedActions);
            int lastCount = node.UnvisitedActions.Count;

            while (lastCount > 0)
            {
                var expanded = node.Expand();

                int newCount = node.UnvisitedActions.Count;
                Assert.That(newCount, Is.LessThan(lastCount));

                TestAction newAction = expanded.IncomingAction;
                Assert.That(originalUnvisitedActions, Has.Member(newAction));
                Assert.That(node.UnvisitedActions, Has.No.Member(newAction));

                lastCount = newCount;
            }
        }

        [Test]
        public void FindSearchCandidateCanExpand()
        {
            var node = Helper.RandomRoot();
            Assert.That(node.Children, Is.Empty);

            var candidate = node.FindSearchCandidate(TestMctsConstants.EXPLORATION_FACTOR);
            if (candidate is ConcreteChild child)
            {
                Assert.That(child.Parent, Is.Not.Null);
            }
            else
            {
                Assert.Fail("candidate should be a child");
            }

            Assert.That(node.Children, Is.Not.Empty);
        }

        [Test]
        public void FindSearchCandidateFindsLeafIfPossible()
        {
            var node1 = Helper.RandomRoot();
            var node11 = Helper.RandomChild(node1);
            var node12 = Helper.RandomChild(node1);
            var leaf111 = Helper.RandomChild(node11);
            var leaf112 = Helper.RandomChild(node11);
            var leaf121 = Helper.RandomChild(node12);

            node1.UnvisitedActions.Clear();
            node11.UnvisitedActions.Clear();
            node12.UnvisitedActions.Clear();

            var candidate = node1.FindSearchCandidate(TestMctsConstants.EXPLORATION_FACTOR);

            Assert.That(candidate.Children, Is.Empty);
            if (candidate is ConcreteChild child)
            {
                Assert.That(child.Parent, Is.Not.Null);
            }
            else
            {
                Assert.Fail("candidate should be child");
            }
        }

        [Test]
        public void FindSearchCandidateReturnsRootIfTerminal()
        {
            var terminalState = new TestState(12);

            var root = new ConcreteNode(terminalState);
            var node = root.FindSearchCandidate(TestMctsConstants.EXPLORATION_FACTOR);
            Assert.That(node, Is.SameAs(root));

            var detached = new ConcreteChild(terminalState, null, new TestAction(5));
            node = detached.FindSearchCandidate(TestMctsConstants.EXPLORATION_FACTOR);
            Assert.That(node, Is.SameAs(detached));
        }

        [Test]
        public void FindSearchCandidateStopsIfBestChildTerminal()
        {
            var parent = Helper.RandomRoot();
            parent.UnvisitedActions.Clear();

            var worstState = new ConcreteChild(new TestState(0), parent, new TestAction(0))
            {
                Visits = 1
            };
            var terminalState = new ConcreteChild(new TestState(10), parent, new TestAction(10))
            {
                Visits = 1,
                Score = 1.0
            };
            terminalState.UnvisitedActions.Clear();
            terminalState.UnvisitedActions.Add(new TestAction(0));

            var candidate = parent.FindSearchCandidate(0);

            Assert.That(candidate, Is.SameAs(terminalState));
            Assert.That(candidate, Is.Not.SameAs(worstState));

            Assert.That(candidate.UnvisitedActions, Has.Count.EqualTo(1));
            Assert.That(candidate.UnvisitedActions, Has.Member(new TestAction(0)));
        }
    }

    public class TestChildSearchTreeNode
    {
        [Test]
        public void BackupBacksUp()
        {
            var node1 = Helper.RandomRoot();
            var node2 = Helper.RandomChild(node1);
            var node3 = Helper.RandomChild(node2);
            var node4 = Helper.RandomChild(node3);

            float score = Random.value;
            node4.Backup(score);

            Assert.That(node4.Score, Is.EqualTo(score));
            Assert.That(node3.Score, Is.EqualTo(-score));
            Assert.That(node2.Score, Is.EqualTo(score));
            Assert.That(node1.Score, Is.EqualTo(-score));
        }

        [Test]
        public void BackupAvoidsUnrelatedNodes()
        {
            var node1 = Helper.RandomRoot();
            var node2 = Helper.RandomChild(node1);
            var node3 = Helper.RandomChild(node2);
            var node4 = Helper.RandomChild(node3);

            var nodeA = Helper.RandomChild(node1);
            var nodeA1 = Helper.RandomChild(nodeA);
            var nodeB = Helper.RandomChild(node2);
            var nodeC = Helper.RandomChild(node3);
            var nodeD = Helper.RandomChild(node4);

            float score = Random.value;
            node4.Backup(score);

            Assert.That(nodeA.Score, Is.EqualTo(0));
            Assert.That(nodeA1.Score, Is.EqualTo(0));
            Assert.That(nodeB.Score, Is.EqualTo(0));
            Assert.That(nodeC.Score, Is.EqualTo(0));
            Assert.That(nodeD.Score, Is.EqualTo(0));
        }

        [Test]
        public void ChildConstructorProperlyHooksUpToParent()
        {
            var parent = Helper.RandomRoot();

            var child = new ChildSearchTreeNode<TestPlayer, TestState, TestAction>(
                new TestState(10),
                parent,
                new TestAction(10)
            );

            Assert.That(parent.Children, Has.Member(child));
            Assert.That(child.Parent, Is.EqualTo(parent));
        }
    }
}
