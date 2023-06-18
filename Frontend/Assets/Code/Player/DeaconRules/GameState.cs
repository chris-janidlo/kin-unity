using System;
using Code.Player.MCTS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Code.Player.DeaconRules
{
    public readonly struct GameState : IGameState<Player, GameState, GameAction>
    {
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

            void Loop(Board workingBoard, Player toPlay, List<ActionComponent> components)
            {
                foreach ((Vector2Int coord, MovementData movement) in _GAA_ACTIVE_PIECES)
                {
                    if (components.Count != 0 && coord == components[0].Origin) // note that FirstOrDefault is not safe to use here because ActionComponent is a value type
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

                                    Board temp = workingBoard.Clone();
                                    Board.ApplyActionComponent(temp.pieces_packed, component);

                                    if (components.Count == _GAA_ACTIVE_PIECES.Count - 1)
                                    {
                                        if (_GAA_SEEN_BOARDS.Contains(temp))
                                            continue;

                                        _GAA_SEEN_BOARDS.Add(temp);

                                        var componentsClone = ListPool<ActionComponent>.Get();
                                        componentsClone.AddRange(components);
                                        componentsClone.Add(component);

                                        buffer.Add(new GameAction(componentsClone));
                                    }
                                    else
                                    {
                                        var componentsClone = ListPool<ActionComponent>.Get();
                                        componentsClone.AddRange(components);
                                        componentsClone.Add(component);

                                        Loop(temp, toPlay, componentsClone);
                                    }
                                }
                            }

                            if (targetedSpace.HasValue)
                                break;
                        }
                    }
                }

                ListPool<ActionComponent>.Release(components);
            }

            Loop(Board.Clone(), toPlay, ListPool<ActionComponent>.Get());

            Debug.Log(buffer.Count);
        }

        public Player NextToPlay()
        {
            return toPlay;
        }

        // TODO: verify that this properly releases lists to pool
        public GameState ApplyAction(GameAction action)
        {
            Board newBoard = Board.ApplyAction(action);
            Player newPlayer = toPlay switch
            {
                Player.Red => Player.Blue,
                Player.Blue => Player.Red,
                _ => throw new ArgumentOutOfRangeException()
            };

            ListPool<ActionComponent>.Release(action.Components);

            return new GameState(newBoard, newPlayer);
        }

        // TODO: verify that this properly releases lists to pool
        public GameAction DefaultPolicy(IReadOnlyList<GameAction> actions)
        {
            int chosenIndex = UnityEngine.Random.Range(0, actions.Count);

            for (var i = 0; i < actions.Count; i++)
            {
                if (i == chosenIndex)
                    continue;

                ListPool<ActionComponent>.Release(actions[i].Components);
            }

            return actions[chosenIndex];
        }

        public double? ValueForPlayer(Player player)
        {
            // assumes 2-player zero-sum

            var selfAlive = false;
            var oppAlive = false;

            foreach (byte packed in Board.pieces_packed)
            {
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
