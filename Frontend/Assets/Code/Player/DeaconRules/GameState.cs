using System;
using Code.Player.MCTS;
using Unity.Collections;
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

        public void GetAvailableActions(ref NativeList<GameAction> buffer)
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

        public GameAction DefaultPolicy(NativeList<GameAction> actions)
        {
            throw new System.NotImplementedException();
        }

        public double? ValueForPlayer(Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}
