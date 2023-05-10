using System;
using System.Collections.Generic;
using System.Linq;
using Code.Player.MCTS;
using NUnit.Framework;
using Random = UnityEngine.Random;

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

        [Test]
        public void BestChildMaximizesUcb1()
        {
            // arbitrary values chosen so that child a has a higher UCB1 when
            // effective_exploration_factor is set to 0, and child b is higher when
            // effective_exploration_factor is set to 1

            var parent = Helper.RandomRoot();
            parent.Score = -1.0;
            parent.Visits = 3;

            var child1State = new TestState(Random.Range(1, 5));
            var child1Action = new TestAction(child1State.Value - parent.GameState.Value);
            var child1 = new ConcreteChild(child1State, parent, child1Action)
            {
                Score = 5.0,
                Visits = 50
            };

            var child2State = new TestState(Random.Range(6, 10));
            var child2Action = new TestAction(child2State.Value - parent.GameState.Value);
            var child2 = new ConcreteChild(child2State, parent, child2Action)
            {
                Score = 1.0,
                Visits = 20
            };

            var bestChildC0 = parent.BestChild(0.0);
            var bestChildC1 = parent.BestChild(1.0);

            Assert.That(bestChildC0, Is.SameAs(child1));
            Assert.That(bestChildC1, Is.SameAs(child2));
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

            var originalScores = new[] { node1, node2, node3, node4 }.Select(n => n.Score).ToList();

            float score = Random.value;
            node4.Backup(score);

            Assert.That(node4.Score, Is.EqualTo(originalScores[3] + score));
            Assert.That(node3.Score, Is.EqualTo(originalScores[2] - score));
            Assert.That(node2.Score, Is.EqualTo(originalScores[1] + score));
            Assert.That(node1.Score, Is.EqualTo(originalScores[0] - score));
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

            var originalScores = new[] { nodeA, nodeA1, nodeB, nodeC, nodeD }
                .Select(n => n.Score)
                .ToList();

            float score = Random.value;
            node4.Backup(score);

            Assert.That(nodeA.Score, Is.EqualTo(originalScores[0]));
            Assert.That(nodeA1.Score, Is.EqualTo(originalScores[1]));
            Assert.That(nodeB.Score, Is.EqualTo(originalScores[2]));
            Assert.That(nodeC.Score, Is.EqualTo(originalScores[3]));
            Assert.That(nodeD.Score, Is.EqualTo(originalScores[4]));
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

        [Test]
        public void DetachDoesNotLeak()
        {
            // https://stackoverflow.com/a/579001/5931898

            ConcreteChild child = null;
            WeakReference reference = null;
            new Action(() =>
            {
                var parent = Helper.RandomRoot();
                child = Helper.RandomChild(parent);

                child.Detach();

                reference = new WeakReference(parent, true);
            })();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.That(child, Is.Not.Null);
            Assert.That(reference, Is.Not.Null);
            Assert.That(reference.Target, Is.Null);
        }
    }
}
