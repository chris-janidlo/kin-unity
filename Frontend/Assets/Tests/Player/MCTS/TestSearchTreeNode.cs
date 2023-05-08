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

            return new ConcreteNode(state);
        }

        internal static ConcreteChild RandomChild(ConcreteNode parent)
        {
            var state = new TestState(Random.Range(0, 10));
            var action = new TestAction(state.Value - parent.GameState.Value);

            return new ConcreteChild(state, parent, action);
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
    }
}
