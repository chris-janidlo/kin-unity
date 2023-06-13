using System.Collections.Generic;
using System.Linq;
using Core_Rules;
using UnityEngine;

namespace MCTS
{
    public class MctsNode
    {
        private readonly List<MctsNode> _children;
        private readonly MctsParameters _searchParameters;

        private MctsNode _parent;

        private float _totalScore;

        public MctsNode(GameState state, MctsParameters searchParameters)
        {
            GameState = state;
            _children = new List<MctsNode>();
            _searchParameters = searchParameters;
        }

        public GameState GameState { get; }
        public int Visits { get; private set; }

        public bool IsLeaf => _children.Count == 0;
        public bool IsTerminalState => GameState.IsLossState;

        public IEnumerable<MctsNode> Children => _children.AsReadOnly();

        public MctsNode ExplorationCandidate()
        {
            var currentCandiate = _children.First();
            var maxExplorationValue = currentCandiate.ExplorationValue();

            foreach (var child in _children.Skip(1))
            {
                var candidateUcb1 = child.ExplorationValue();
                if (candidateUcb1 > maxExplorationValue)
                {
                    currentCandiate = child;
                    maxExplorationValue = candidateUcb1;
                }
            }

            return currentCandiate;
        }

        public MctsNode ChildWithState(GameState state)
        {
            return _children.FirstOrDefault(n => n.GameState == state);
        }

        public MctsNode BestChild()
        {
            var bestChild = _children.First();
            float maxVisits = bestChild.Visits;

            foreach (var child in _children.Skip(1))
                if (child.Visits > maxVisits)
                {
                    bestChild = child;
                    maxVisits = child.Visits;
                }

            return bestChild;
        }

        public void Expand()
        {
            foreach (var state in GameState.LegalFutureStates())
                AddChild(new MctsNode(state, _searchParameters));
        }

        public void BackPropagate(float score)
        {
            _totalScore += score;
            Visits++;
            _parent?.BackPropagate(score);
        }

        public void Detach()
        {
            _parent = null;
        }

        private void AddChild(MctsNode child)
        {
            child._parent = this;
            _children.Add(child);
        }

        // UCT formula
        private float ExplorationValue()
        {
            if (Visits == 0)
                return float.PositiveInfinity;

            var value =
                // prefer nodes with higher score:
                _totalScore / Visits
                // but also look at nodes that have been under-explored:
                + _searchParameters.explorationFactor
                    * Mathf.Sqrt(Mathf.Log(_parent.Visits) / Visits);

            return value;
        }
    }
}
