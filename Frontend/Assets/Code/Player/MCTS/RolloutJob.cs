using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Code.Player.MCTS
{
    [BurstCompile]
    public struct RolloutJob<TPlayer, TState, TAction> : IJob
        where TPlayer : Enum
        where TState : struct, IGameState<TPlayer, TState, TAction>
        where TAction : unmanaged, IGameAction
    {
        public TState StartState;
        public TPlayer ForPlayer;

        public NativeList<TAction> ActionBuffer;

        public NativeArray<double> Result;

        public void Execute()
        {
            var currentState = StartState;

            var result = currentState.ValueForPlayer(ForPlayer);

            while (result == null)
            {
                ActionBuffer.Clear();
                currentState.GetAvailableActions(ref ActionBuffer);
                var action = currentState.DefaultPolicy(ActionBuffer);
                currentState = currentState.ApplyAction(action);
                result = currentState.ValueForPlayer(ForPlayer);
            }

            Result[0] = result.Value;
        }
    }
}