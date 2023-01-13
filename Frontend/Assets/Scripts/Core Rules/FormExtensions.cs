using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core_Rules
{
    public static class FormExtensions
    {
        private static readonly Vector2Int[] CardinalDirections = new Vector2Int[4]
        {
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
        };

        private static readonly Vector2Int[] DiagonalDirections = new Vector2Int[4]
        {
            new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
        };

        private static readonly Vector2Int[] AllDirections = new Vector2Int[8]
        {
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
            new(1, 1), new(1, -1), new(-1, -1), new(-1, 1)
        };

        private static readonly Dictionary<Form, IEnumerable<Form>> TransitionMap = new()
        {
            [Form.Captain] = new List<Form>
            {
                Form.Scientist
            },
            [Form.Engineer] = new List<Form>
            {
                Form.Pilot,
                Form.Priest
            },
            [Form.Pilot] = new List<Form>
            {
                Form.Engineer,
                Form.Priest,
                Form.Captain
            },
            [Form.Priest] = new List<Form>
            {
                Form.Robot,
                Form.Engineer
            },
            [Form.Robot] = new List<Form>
            {
                Form.Engineer,
                Form.Priest,
                Form.Captain
            },
            [Form.Scientist] = new List<Form>
            {
                Form.Engineer,
                Form.Priest
            }
        };

        public static IEnumerable<Vector2Int> GetLegalBoardPositions(this Form form, Vector2Int currentPosition,
            Board board)
        {
            return form switch
            {
                Form.Captain => Movement(currentPosition, board, 5, AllDirections, PieceInteraction.Capture),
                Form.Engineer => Movement(currentPosition, board, 2, DiagonalDirections, PieceInteraction.Swap),
                Form.Pilot => Movement(currentPosition, board, 2, DiagonalDirections, PieceInteraction.Capture),
                Form.Priest => Movement(currentPosition, board, 3, CardinalDirections, PieceInteraction.Swap),
                Form.Robot => Movement(currentPosition, board, 2, CardinalDirections, PieceInteraction.Capture),
                Form.Scientist => Movement(currentPosition, board, 1, AllDirections, PieceInteraction.None),
                _ => throw new ArgumentException($"unexpected {form.GetType().Name} value {form}")
            };
        }

        public static IEnumerable<Form> GetFormTransitions(this Form form)
        {
            return TransitionMap[form];
        }

        public static PieceInteraction GetInteraction(this Form form)
        {
            return form switch
            {
                Form.Captain => PieceInteraction.Capture,
                Form.Engineer => PieceInteraction.Swap,
                Form.Pilot => PieceInteraction.Capture,
                Form.Priest => PieceInteraction.Swap,
                Form.Robot => PieceInteraction.Capture,
                Form.Scientist => PieceInteraction.None,
                _ => throw new ArgumentException($"unexpected {form.GetType().Name} value {form}")
            };
        }

        private static IEnumerable<Vector2Int> Movement(Vector2Int currentPosition, Board board, int range,
            Vector2Int[] directions, PieceInteraction pieceInteraction)
        {
            foreach (var dir in directions)
                for (var r = 1; r <= range; r++)
                {
                    var candidatePosition = currentPosition + dir * r;
                    if (!board.InBounds(candidatePosition)) break;

                    var pieceAtTarget = board.GetPiece(candidatePosition);

                    if (pieceAtTarget == null)
                    {
                        yield return candidatePosition;
                        // if there isn't a piece there, we don't need to worry about PieceInteractions; keep searching
                        // in this direction:
                        continue;
                    }

                    var thisOwner = board.GetPiece(currentPosition).Value.Owner;

                    if (pieceInteraction != PieceInteraction.None && pieceAtTarget.Value.Owner != thisOwner)
                        yield return candidatePosition;

                    break; // no piece can jump over other pieces
                }
        }
    }
}
