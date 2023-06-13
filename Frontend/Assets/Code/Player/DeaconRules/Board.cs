using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Code.Player.DeaconRules
{
    public readonly struct Board
    {
        public const int DIMENSION = 5;

        public static readonly Board EMPTY = new Board(Enumerable.Empty<(int, int, Piece)>());

        public static readonly Board INITIAL = new Board(
            new[]
            {
                (1, 0, new Piece(Form.Scientist, Player.Blue)),
                (0, 1, new Piece(Form.Scientist, Player.Blue)),
                (4, 3, new Piece(Form.Scientist, Player.Red)),
                (3, 4, new Piece(Form.Scientist, Player.Red)),
            }
        );

        private readonly Piece?[] pieces;

        // [0, 0] is top left of board
        // increasing x coordinates go toward the right
        // increasing y coordinates go downward
        public Piece? this[int x, int y] => pieces[C(x, y)];
        public Piece? this[Vector2Int coordinate] => pieces[C(coordinate)];

        public Board(IEnumerable<(int, int, Piece)> occupiedSpaces)
        {
            pieces = new Piece?[DIMENSION * DIMENSION];

            foreach ((int x, int y, Piece piece) in occupiedSpaces)
                pieces[x + y * DIMENSION] = piece;
        }

        public Board(string layout)
        {
            pieces = new Piece?[DIMENSION * DIMENSION];

            char TryParseCharFromString(string s)
            {
                try
                {
                    return char.Parse(s);
                }
                catch (FormatException ex)
                {
                    var msg = $"{nameof(layout)} must be all chars\n\ngot:\n{layout}";
                    throw new ArgumentException(msg, ex);
                }
            }

            char[][] chars = layout
                .Split(Environment.NewLine)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(line =>
                {
                    return line.Split()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(TryParseCharFromString)
                        .ToArray();
                })
                .ToArray();

            var lengths = chars.Select(r => r.Length);
            if (chars.Length != DIMENSION || lengths.Any(l => l != DIMENSION))
            {
                string msg =
                    $"{nameof(layout)} must be a {DIMENSION}x{DIMENSION} matrix"
                    + $"\n\ngot:\n{layout}"
                    + $"\n\n(lengths: [{string.Join(", ", lengths)}])";

                throw new ArgumentException(msg);
            }

            for (var x = 0; x < DIMENSION; x++)
            {
                for (var y = 0; y < DIMENSION; y++)
                {
                    char c = chars[y][x];

                    if (c == '_')
                        continue;

                    Form form = char.ToLower(c) switch
                    {
                        'c' => Form.Captain,
                        'e' => Form.Engineer,
                        'i' => Form.Pilot,
                        'p' => Form.Priest,
                        'r' => Form.Robot,
                        's' => Form.Scientist,
                        _ => throw new ArgumentException($"unexpected form character {c}")
                    };

                    Player owner = char.IsUpper(c) ? Player.Blue : Player.Red;

                    pieces[C(x, y)] = new Piece(form, owner);
                }
            }
        }

        private Board(Piece?[] pieces)
        {
            this.pieces = pieces;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var y = 0; y < DIMENSION; y++)
            {
                for (var x = 0; x < DIMENSION; x++)
                {
                    var piece = this[x, y];

                    if (piece == null)
                    {
                        sb.Append("_ ");
                        continue;
                    }

                    sb.Append(piece.Value.ToChar());
                    sb.Append(' ');
                }

                sb.Length--; // remove trailing space
                sb.Append('\n');
            }

            sb.Length--; // remove trailing newline

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is Board other && Equals(other);
        }

        public bool Equals(Board other)
        {
            return pieces.SequenceEqual(other.pieces);
        }

        public override int GetHashCode()
        {
            return pieces.GetHashCode();
        }

        internal static void ApplyActionComponent(ref Piece?[] array, ActionComponent component)
        {
            // ReSharper disable once PossibleInvalidOperationException
            Piece original = array[C(component.Origin)].Value;

            var newPieceAtOrigin =
                original.Movement.Interaction == Interaction.Swap
                    ? array[C(component.Destination)]
                    : null;

            var newPieceAtDestination = new Piece(component.Transformation, original.Owner);

            array[C(component.Origin)] = newPieceAtOrigin;
            array[C(component.Destination)] = newPieceAtDestination;
        }

        internal Board ApplyAction(GameAction action)
        {
            var clone = (Piece?[])pieces.Clone();

            ApplyActionComponent(ref clone, action.First);
            ApplyActionComponent(ref clone, action.Second);

            return new Board(clone);
        }

        private static int C(int x, int y) => x + y * DIMENSION;

        private static int C(Vector2Int coordinate) => C(coordinate.x, coordinate.y);
    }
}
