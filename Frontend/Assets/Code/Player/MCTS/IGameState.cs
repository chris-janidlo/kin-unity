using System;
using JetBrains.Annotations;
using Unity.Collections;

namespace Code.Player.MCTS
{
    public interface IGameState<TPlayer, out TState, TAction>
        where TPlayer : Enum
        where TState : struct, IGameState<TPlayer, TState, TAction>
        where TAction : unmanaged, IGameAction
    {
        public int ActionArrayMaxSize { get; }

        public void GetAvailableActions(ref NativeList<TAction> buffer);

        [Pure]
        public TPlayer NextToPlay();

        [Pure]
        public TState ApplyAction(TAction action);

        [Pure]
        public TAction DefaultPolicy(NativeList<TAction> actions);

        [Pure]
        public double? ValueForPlayer(TPlayer player);
    }
}
