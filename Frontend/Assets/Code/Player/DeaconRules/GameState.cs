using System;
using Code.Player.MCTS;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Player.DeaconRules
{
    public readonly struct GameState : IGameState<Player, GameState, GameAction>
    {
        public int ActionArrayMaxSize => 300; // TODO: find better bound for this

        public Board Board { get; }

        private readonly Player toPlay;

        private GameState(Board board, Player toPlay)
        {
            Board = board;
            this.toPlay = toPlay;
        }

        public void GetAvailableActions(IList<GameAction> buffer)
        {
            throw new System.NotImplementedException();
        }

        public Player NextToPlay()
        {
            return toPlay;
        }

        public GameState ApplyAction(GameAction action)
        {
            Board newBoard = Board.ApplyAction(action);
            Player newPlayer = toPlay switch
            {
                Player.Red => Player.Blue,
                Player.Blue => Player.Red,
                _ => throw new ArgumentOutOfRangeException()
            };

            return new GameState(newBoard, newPlayer);
        }

        public GameAction DefaultPolicy(IReadOnlyList<GameAction> actions)
        {
            return actions[UnityEngine.Random.Range(0, actions.Count)];
        }

        public double? ValueForPlayer(Player player)
        {
            return Board.ValueForPlayer(player);
        }
    }
}
