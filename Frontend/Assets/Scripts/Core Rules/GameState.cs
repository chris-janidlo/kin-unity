using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core_Rules
{
    public struct GameState : IEquatable<GameState>
    {
        private const Player FirstPlayer = Player.Blue;

        public Board Board;
        public Player CurrentPlayer;
        public bool IsLossState;

        public static GameState InitialGameState()
        {
            var board = Board.CreateBoard();

            void AddPiece(Player owner, char id, Vector2Int position)
            {
                var piece = new Piece(id, owner, Form.Scientist, position);
                board.SetPiece(piece, position);
            }

            AddPiece(Player.Blue, 'B', new Vector2Int(1, 0));
            AddPiece(Player.Blue, 'b', new Vector2Int(0, 1));

            const int redCorner = Board.SideLength - 1;

            AddPiece(Player.Red, 'R', new Vector2Int(redCorner - 1, redCorner));
            AddPiece(Player.Red, 'r', new Vector2Int(redCorner, redCorner - 1));

            return new GameState { Board = board, CurrentPlayer = FirstPlayer };
        }

        public static bool operator ==(GameState lhs, GameState rhs)
        {
            return lhs.GetHashCode() == rhs.GetHashCode();
        }

        public static bool operator !=(GameState lhs, GameState rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return (Board, CurrentPlayer, IsLossState).GetHashCode();
        }

        public bool Equals(GameState other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is GameState other && this == other;
        }

        public IEnumerable<GameState> LegalFutureStates()
        {
            var seenStates = new HashSet<GameState>();
            var pieces = PiecesOwnedByCurrentPlayer().ToList();

            if (pieces.Count == 1)
            {
                var piece = pieces[0];

                foreach (var move in piece.LegalMoves(Board))
                {
                    var state = ApplyMove(this, piece, move, true);
                    if (seenStates.Contains(state))
                        continue;

                    seenStates.Add(state);
                    yield return state;
                }

                yield break;
            }

            foreach (var piece1 in pieces)
                foreach (var move1 in piece1.LegalMoves(Board))
                {
                    var intermediateState = ApplyMove(this, piece1, move1, false);
                    foreach (var piece2 in pieces)
                    {
                        if (piece1 == piece2)
                            continue;

                        foreach (var move2 in piece2.LegalMoves(intermediateState.Board))
                        {
                            var finalState = ApplyMove(intermediateState, piece2, move2, true);
                            if (seenStates.Contains(finalState))
                                continue;

                            seenStates.Add(finalState);
                            yield return finalState;
                        }
                    }
                }
        }

        public IEnumerable<Vector2Int> LegalPositionsForPieceAt(Vector2Int position)
        {
            if (Board.GetPiece(position) is not { } piece)
                throw new ArgumentException($"there is no piece at {position}");

            return piece.Form.GetLegalBoardPositions(position, Board);
        }

        public IEnumerable<Form> LegalFormTransitionsForPieceAt(Vector2Int position)
        {
            if (Board.GetPiece(position) is not { } piece)
                throw new ArgumentException($"there is no piece at {position}");

            return piece.Form.GetFormTransitions();
        }

        public IEnumerable<Piece> PiecesOwnedByCurrentPlayer()
        {
            return Board.GetPiecesByOwner(CurrentPlayer);
        }

        public void FlipPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.Blue ? Player.Red : Player.Blue;
        }

        public Piece? GetPieceById(char id)
        {
            return Board.GetPieceById(id);
        }

        // assumes move is legal
        public static GameState ApplyMove(
            GameState originalState,
            Piece oldPiece,
            Piece newPiece,
            bool changePlayer
        )
        {
            var result = originalState;
            // since the above line results in a fast memory copy, we need to create a new Board in order to not
            // overwrite the one in originalState:
            result.Board = Board.DeepClone(originalState.Board);

            if (
                result.Board.GetPiece(newPiece.Position) is { } conflictingPiece
                && oldPiece.Form.GetInteraction() == PieceInteraction.Swap
            )
            {
                conflictingPiece.Position = oldPiece.Position;
                result.Board.SetPiece(conflictingPiece, oldPiece.Position);
            }
            else
            {
                result.Board.RemovePiece(oldPiece.Position);
            }

            result.Board.SetPiece(newPiece, newPiece.Position);

            if (changePlayer)
                result.FlipPlayer();

            result.IsLossState = result.Board.IsLossForPlayer(result.CurrentPlayer);

            return result;
        }
    }
}
