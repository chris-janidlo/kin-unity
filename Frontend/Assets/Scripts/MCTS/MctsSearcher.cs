using System;
using System.Collections.Generic;
using System.Linq;
using Core_Rules;

namespace MCTS
{
    public class MctsSearcher
    {
        private readonly Random _random;

        private readonly MctsParameters _searchParameters;

        private MctsNode _previousChoice;

        public MctsSearcher(Player player, MctsParameters searchParameters)
        {
            Player = player;
            _random = new Random();
            _searchParameters = searchParameters;
        }

        public Player Player { get; }

        public GameState Search(GameState startingState, Action<float> setProgress)
        {
            var root = _previousChoice?.ChildWithState(startingState) ?? new MctsNode(startingState, _searchParameters);
            root.Detach();
            root.Expand(); // always need at least one level expanded from the root

            for (var i = 0; i < _searchParameters.iterations; i++)
            {
                var leaf = Traverse(root); // selection and expansion
                var score = Rollout(leaf); // rollout
                leaf.BackPropagate(score); // back propagation
                setProgress((float)i / _searchParameters.iterations);
            }

            _previousChoice = root.BestChild();
            return _previousChoice.GameState;
        }

        private static MctsNode Traverse(MctsNode root)
        {
            var node = root;
            while (!node.IsLeaf) node = node.ExplorationCandidate();

            // terminal nodes are essentially rollouts that are decided instantly.
            // https://ai.stackexchange.com/a/6993/53886 has ideas for improvements here
            if (node.Visits == 0 || node.IsTerminalState) return node;

            node.Expand();
            // technically all children will have the highest possible exploration score of infinity due to never
            // having been explored, so skip the calculation and just grab the first one
            return node.Children.First();
        }

        private float Rollout(MctsNode node)
        {
            var state = node.GameState;
            while (!state.IsLossState) state = RolloutPolicy(state.LegalFutureStates());

            return state.CurrentPlayer == Player
                ? -1
                : 1;
        }

        private GameState RolloutPolicy(IEnumerable<GameState> options)
        {
            return options.ElementAt(_random.Next(options.Count()));
        }
    }
}
