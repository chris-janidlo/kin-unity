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

        public GameState(Board board, Player toPlay)
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
            // assumes 2-player zero-sum

            bool selfAlive = false;
            bool oppAlive = false;

            foreach (var packed in Board.pieces_packed)
            {
                if (!(Piece.Unpack(packed) is { } piece))
                    continue;

                if (piece.Owner == player)
                {
                    selfAlive = true;
                }
                else
                {
                    oppAlive = true;
                }

                if (selfAlive && oppAlive)
                    return null;
            }

            return selfAlive ? 1 : -1;
        }
    }
}
