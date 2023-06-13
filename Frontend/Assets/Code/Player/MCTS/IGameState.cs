using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Code.Player.MCTS
{
    public interface IGameState<TPlayer, out TState, TAction>
        where TPlayer : Enum
        where TState : struct, IGameState<TPlayer, TState, TAction>
        where TAction : unmanaged, IGameAction
    {
        public int ActionArrayMaxSize { get; }

        public void GetAvailableActions(IList<TAction> buffer);

        [Pure]
        public TPlayer NextToPlay();

        [Pure]
        public TState ApplyAction(TAction action);

        [Pure]
        public TAction DefaultPolicy(IReadOnlyList<TAction> actions);

        [Pure]
        public double? ValueForPlayer(TPlayer player);
    }
}
