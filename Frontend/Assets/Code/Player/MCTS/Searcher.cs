using System;
using System.Collections.Generic;

namespace Code.Player.MCTS
{
    // TODO: allow pre-searching on actions the player hasn't committed to yet
    public class Searcher<TPlayer, TState, TAction>
        where TPlayer : Enum
        where TState : struct, IGameState<TPlayer, TState, TAction>
        where TAction : unmanaged, IGameAction
    {
        internal readonly SearchParameters DefaultParameters;
        internal SearchTreeNode<TPlayer, TState, TAction> Tree;

        public Searcher(TState rootState, SearchParameters defaultParameters)
        {
            Tree = new SearchTreeNode<TPlayer, TState, TAction>(rootState);
            DefaultParameters = defaultParameters;
        }

        public TAction Search(SearchParameters? parameters = null)
        {
            SearchParameters searchParameters = parameters ?? DefaultParameters;

            List<TAction> rolloutBuffer = new List<TAction>(Tree.GameState.ActionArrayMaxSize);
            TPlayer rootPlayer = Tree.GameState.NextToPlay();

            for (var _ = 0; _ < searchParameters.Iterations; _++)
            {
                var leaf = Tree.FindSearchCandidate(searchParameters.ExplorationFactor);
                double score = Rollout(leaf.GameState, rootPlayer, rolloutBuffer);
                if (leaf is ChildSearchTreeNode<TPlayer, TState, TAction> child)
                    child.Backup(score);
            }

            var choice = Tree.BestChild(0.0);
            choice.Detach();
            Tree = choice;

            return choice.IncomingAction;
        }

        public void ApplyAction(TAction action)
        {
            var childWithAction = Tree.ChildWithAction(action);

            if (childWithAction == null)
            {
                TState newState = Tree.GameState.ApplyAction(action);
                Tree = new SearchTreeNode<TPlayer, TState, TAction>(newState);
            }
            else
            {
                childWithAction.Detach();
                Tree = childWithAction;
            }
        }

        private static double Rollout(TState fromState, TPlayer forPlayer, List<TAction> buffer)
        {
            TState currentState = fromState;
            double? result;

            while ((result = currentState.ValueForPlayer(forPlayer)) == null)
            {
                buffer.Clear();
                currentState.GetAvailableActions(buffer);
                var action = currentState.DefaultPolicy(buffer);
                currentState = currentState.ApplyAction(action);
            }

            return result.Value;
        }
    }
}
