using System;
using System.Collections.Generic;
using System.Linq;

namespace Code.Player.MCTS
{
    internal class SearchTreeNode<TPlayer, TState, TAction>
        where TPlayer : Enum
        where TState : struct, IGameState<TPlayer, TState, TAction>
        where TAction : struct, IGameAction
    {
        public readonly List<ChildSearchTreeNode<TPlayer, TState, TAction>> Children;

        public readonly TState GameState;
        public readonly List<TAction> UnvisitedActions;

        public double Score;
        public int Visits;

        public SearchTreeNode(TState state)
        {
            Children = new List<ChildSearchTreeNode<TPlayer, TState, TAction>>();

            GameState = state;

            UnvisitedActions = new List<TAction>(state.ActionArrayMaxSize);
            state.GetAvailableActions(UnvisitedActions);

            Score = 0.0;
            Visits = 0;
        }

        public SearchTreeNode<TPlayer, TState, TAction> FindSearchCandidate(
            double explorationFactor
        )
        {
            return TreePolicy(this, explorationFactor);
        }

        public ChildSearchTreeNode<TPlayer, TState, TAction> BestChild(double explorationFactor)
        {
            ChildSearchTreeNode<TPlayer, TState, TAction> bestChild = null;
            double bestUcb1Score = double.NegativeInfinity;

            double parentVisitTerm = 2.0 * Math.Log(Visits);

            foreach (var child in Children)
            {
                double exploitationTerm = child.Score / child.Visits;
                double explorationTerm = Math.Sqrt(parentVisitTerm / child.Visits);

                double ucb1 = exploitationTerm + explorationFactor * explorationTerm;

                if (ucb1 > bestUcb1Score)
                {
                    bestUcb1Score = ucb1;
                    bestChild = child;
                }

                // TODO: break ties randomly? currently biased toward earlier moves
            }

            if (bestChild == null)
                throw new NullReferenceException();

            return bestChild;
        }

        public ChildSearchTreeNode<TPlayer, TState, TAction> Expand()
        {
            var action = GameState.DefaultPolicy(UnvisitedActions);
            UnvisitedActions.Remove(action);

            TState newState = GameState.ApplyAction(action);
            return new ChildSearchTreeNode<TPlayer, TState, TAction>(newState, this, action);
        }

        public ChildSearchTreeNode<TPlayer, TState, TAction> ChildWithAction(TAction action)
        {
            return Children.FirstOrDefault(c => c.IncomingAction.Equals(action));
        }

        private static SearchTreeNode<TPlayer, TState, TAction> TreePolicy(
            SearchTreeNode<TPlayer, TState, TAction> root,
            double explorationFactor
        )
        {
            var candidate = root;
            TPlayer rootPlayer = root.GameState.NextToPlay();

            while (candidate.GameState.ValueForPlayer(rootPlayer) == null)
                if (candidate.UnvisitedActions.Count > 0)
                    return candidate.Expand();
                else
                    candidate = candidate.BestChild(explorationFactor);

            return candidate;
        }

        private static List<TAction> __action_buffer;

        private static List<TAction> GetActionBuffer(TState state)
        {
            if (__action_buffer == null)
            {
                __action_buffer = new List<TAction>(state.ActionArrayMaxSize);
            }
            else
            {
                __action_buffer.Clear();
            }

            return __action_buffer;
        }
    }

    internal class ChildSearchTreeNode<TPlayer, TState, TAction>
        : SearchTreeNode<TPlayer, TState, TAction>
        where TPlayer : Enum
        where TState : struct, IGameState<TPlayer, TState, TAction>
        where TAction : struct, IGameAction
    {
        public TAction IncomingAction;
        public SearchTreeNode<TPlayer, TState, TAction> Parent;

        public ChildSearchTreeNode(
            TState state,
            SearchTreeNode<TPlayer, TState, TAction> parent,
            TAction incomingAction
        )
            : base(state)
        {
            Parent = parent;
            parent?.Children.Add(this);

            IncomingAction = incomingAction;
        }

        public void Backup(double score)
        {
            // TODO: handle non-two-player case?

            BackupNegamax(this, score);
        }

        public void Detach()
        {
            Parent.Children.Remove(this);
            Parent = null;
        }

        private static void BackupNegamax(
            SearchTreeNode<TPlayer, TState, TAction> leaf,
            double score
        )
        {
            var currentNode = leaf;

            while (currentNode != null)
            {
                currentNode.Score += score;
                currentNode.Visits++;

                score = -score;

                if (currentNode is ChildSearchTreeNode<TPlayer, TState, TAction> child)
                    currentNode = child.Parent;
                else
                    return;
            }
        }
    }
}
