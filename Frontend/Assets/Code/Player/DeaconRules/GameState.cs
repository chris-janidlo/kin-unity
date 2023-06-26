using System;
using Code.Player.MCTS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Code.Player.DeaconRules
{
    public readonly struct GameState : IGameState<Player, GameState, GameAction>
    {
        public static readonly GameState INITIAL_STATE = new(Board.INITIAL, Player.Blue);

        public int ActionArrayMaxSize => 300; // TODO: find better bound for this

        public Board Board { get; }

        private readonly Player toPlay;

        private static readonly object _GAA_LOCK = new();
        private static readonly List<(Vector2Int, MovementData)> _GAA_ACTIVE_PIECES = new();
        private static readonly HashSet<Board> _GAA_SEEN_BOARDS = new();

        public GameState(Board board, Player toPlay)
        {
            Board = board;
            this.toPlay = toPlay;
        }

        public void GetAvailableActions(IList<GameAction> buffer)
        {
            lock (_GAA_LOCK)
                GetAvailableActionsThreadUnsafe(buffer);
        }

        private void GetAvailableActionsThreadUnsafe(IList<GameAction> buffer)
        {
            _GAA_ACTIVE_PIECES.Clear();
            _GAA_SEEN_BOARDS.Clear();

            for (var x = 0; x < Board.DIMENSION; x++)
            {
                for (var y = 0; y < Board.DIMENSION; y++)
                {
                    if (Board[x, y] is { } piece && piece.Owner == toPlay)
                    {
                        _GAA_ACTIVE_PIECES.Add((new Vector2Int(x, y), piece.Movement));
                    }
                }
            }

            // assumes there can always either be 1 or 2 active pieces
            bool onePiece = _GAA_ACTIVE_PIECES.Count == 1;

            void Loop(Board workingBoard, Player toPlay, ActionComponent? first = null)
            {
                foreach ((Vector2Int coord, MovementData movement) in _GAA_ACTIVE_PIECES)
                {
                    if (first?.Origin == coord)
                        continue;

                    foreach (Vector2Int direction in movement.Directions)
                    {
                        for (var i = 1; i <= movement.Range; i++)
                        {
                            Vector2Int dest = coord + direction * i;
                            if (
                                dest.x is < 0 or >= Board.DIMENSION
                                || dest.y is < 0 or >= Board.DIMENSION
                            )
                            {
                                break;
                            }

                            var targetedSpace = workingBoard[dest];

                            bool validMove =
                                targetedSpace is not { } targetedPiece
                                || (
                                    movement.Interaction != Interaction.None
                                    && targetedPiece.Owner != toPlay
                                );

                            if (validMove)
                            {
                                foreach (Form transformation in movement.Transformations)
                                {
                                    var component = new ActionComponent(
                                        coord,
                                        dest,
                                        transformation
                                    );

                                    Board.Array piecesPacked = workingBoard.pieces_packed;
                                    Board.ApplyActionComponent(ref piecesPacked, component);
                                    var temp = new Board(piecesPacked);

                                    if (onePiece || first.HasValue)
                                    {
                                        if (_GAA_SEEN_BOARDS.Contains(temp))
                                            continue;

                                        _GAA_SEEN_BOARDS.Add(temp);

                                        GameAction action = first is { } nonNull
                                            ? new GameAction(nonNull, component)
                                            : new GameAction(component);
                                        buffer.Add(action);
                                    }
                                    else
                                    {
                                        Loop(temp, toPlay, component);
                                    }
                                }
                            }

                            if (targetedSpace.HasValue)
                                break;
                        }
                    }
                }
            }

            Loop(Board, toPlay);
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

            var selfAlive = false;
            var oppAlive = false;

            for (var i = 0; i < Board.pieces_packed.Length; i++)
            {
                byte packed = Board.pieces_packed[i];

                if (Piece.Unpack(packed) is not { } piece)
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
