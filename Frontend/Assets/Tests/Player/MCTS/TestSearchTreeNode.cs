using System.Collections.Generic;
using Code.Player.MCTS;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Player.MCTS
{
    using ConcreteNode = SearchTreeNode<TestPlayer, TestState, TestAction>;
    using ConcreteChild = ChildSearchTreeNode<TestPlayer, TestState, TestAction>;

    public class TestSearchTreeNode
    {
        private static ConcreteNode RandomRoot()
        {
            var state = new TestState(Random.Range(0, 10));

            return new ConcreteNode(state);
        }

        private static ConcreteChild RandomChild(ConcreteNode parent)
        {
            var state = new TestState(Random.Range(0, 10));
            var action = new TestAction(state.Value - parent.GameState.Value);

            return new ConcreteChild(state, parent, action);
        }

        [Test]
        public void BackupBacksUp()
        {
            var node1 = RandomRoot();
            var node2 = RandomChild(node1);
            var node3 = RandomChild(node2);
            var node4 = RandomChild(node3);

            float score = Random.value;
            node4.Backup(score);

            Assert.AreEqual(score, node4.Score);
            Assert.AreEqual(-score, node3.Score);
            Assert.AreEqual(score, node2.Score);
            Assert.AreEqual(-score, node1.Score);
        }

        [Test]
        public void BackupAvoidsUnrelatedNodes()
        {
            var node1 = RandomRoot();
            var node2 = RandomChild(node1);
            var node3 = RandomChild(node2);
            var node4 = RandomChild(node3);

            var nodeA = RandomChild(node1);
            var nodeA1 = RandomChild(nodeA);
            var nodeB = RandomChild(node2);
            var nodeC = RandomChild(node3);
            var nodeD = RandomChild(node4);

            float score = Random.value;
            node4.Backup(score);

            Assert.AreEqual(0, nodeA.Score);
            Assert.AreEqual(0, nodeA1.Score);
            Assert.AreEqual(0, nodeB.Score);
            Assert.AreEqual(0, nodeC.Score);
            Assert.AreEqual(0, nodeD.Score);
        }

        [Test]
        public void ExpandAddsNewNodeAsChildOfOriginal()
        {
            var parent = RandomRoot();
            var expanded = parent.Expand();

            Assert.That(parent.Children, Has.Member(expanded));
            Assert.That(expanded.Parent, Is.EqualTo(parent));
        }

        [Test]
        public void ExpandFindsMoveToExpandOn()
        {
            var parent = RandomRoot();
            var node = RandomChild(parent);

            var expanded = node.Expand();

            Assert.That(expanded, Is.Not.EqualTo(node));
        }

        [Test]
        public void ExpandConsumesUnvisitedActions()
        {
            var node = RandomRoot();

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
}
