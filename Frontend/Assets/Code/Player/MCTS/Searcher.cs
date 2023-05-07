using System;
using Unity.Collections;
using Unity.Jobs;

namespace Code.Player.MCTS
{
    // TODO: allow pre-searching on actions the player hasn't committed to yet
    public class Searcher<TPlayer, TState, TAction>
        where TPlayer : Enum
        where TState : struct, IGameState<TPlayer, TState, TAction>
        where TAction : unmanaged, IGameAction
    {
        private readonly SearchParameters defaultParameters;
        private SearchTreeNode<TPlayer, TState, TAction> tree;

        public Searcher(TState rootState, SearchParameters parameters)
            : this(new SearchTreeNode<TPlayer, TState, TAction>(rootState), parameters)
        {
        }

        internal Searcher(SearchTreeNode<TPlayer, TState, TAction> startTree, SearchParameters parameters)
        {
            tree = startTree;
            defaultParameters = parameters;
        }

        public TAction Search(SearchParameters? parameters = null)
        {
            var searchParameters = parameters ?? defaultParameters;

            using var actionBuffer = new NativeList<TAction>(tree.GameState.ActionArrayMaxSize, Allocator.Persistent);
            using var rolloutResults = new NativeArray<double>(1, Allocator.Persistent);

            var rootPlayer = tree.GameState.NextToPlay();

            for (var _ = 0; _ < searchParameters.Iterations; _++)
            {
                var leaf = tree.FindSearchCandidate(searchParameters.ExplorationFactor);
                var score = Rollout(leaf.GameState, rootPlayer, actionBuffer, rolloutResults);
                if (leaf is ChildSearchTreeNode<TPlayer, TState, TAction> child)
                    child.Backup(score);
            }

            return tree.BestChild(0.0).IncomingAction;
        }

        public void ApplyAction(TAction action)
        {
            var newRoot = tree.ChildWithAction(action);
            if (newRoot == null)
            {
                var newState = tree.GameState.ApplyAction(action);
                newRoot = new SearchTreeNode<TPlayer, TState, TAction>(newState);
            }

            tree = newRoot;
        }

        private static double Rollout(TState fromState, TPlayer forPlayer, NativeList<TAction> actionBuffer,
            NativeArray<double> resultContainer)
        {
            var jobData = new RolloutJob<TPlayer, TState, TAction>
            {
                StartState = fromState,
                ForPlayer = forPlayer,
                ActionBuffer = actionBuffer,
                Result = resultContainer
            };

            var jobHandle = jobData.Schedule();
            jobHandle.Complete();

            return resultContainer[0];
        }
    }
}