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

            using var actionBuffer = new NativeList<TAction>(
                Tree.GameState.ActionArrayMaxSize,
                Allocator.Persistent
            );
            using var rolloutResults = new NativeArray<double>(1, Allocator.Persistent);

            TPlayer rootPlayer = Tree.GameState.NextToPlay();

            for (var _ = 0; _ < searchParameters.Iterations; _++)
            {
                var leaf = Tree.FindSearchCandidate(searchParameters.ExplorationFactor);
                double score = Rollout(leaf.GameState, rootPlayer, actionBuffer, rolloutResults);
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

        private static double Rollout(
            TState fromState,
            TPlayer forPlayer,
            NativeList<TAction> actionBuffer,
            NativeArray<double> resultContainer
        )
        {
            var jobData = new RolloutJob<TPlayer, TState, TAction>
            {
                StartState = fromState,
                ForPlayer = forPlayer,
                ActionBuffer = actionBuffer,
                Result = resultContainer
            };

            JobHandle jobHandle = jobData.Schedule();
            jobHandle.Complete();

            return resultContainer[0];
        }
    }
}
