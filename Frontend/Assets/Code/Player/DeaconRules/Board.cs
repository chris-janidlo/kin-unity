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

        // TODO: move away from array since it incurs GC cost. need 25 (half)bytes of data, and could easily hardcode that
        internal readonly byte[] pieces_packed;

        // [0, 0] is top left of board
        // increasing x coordinates go toward the right
        // increasing y coordinates go downward
        public Piece? this[int x, int y] => Piece.Unpack(pieces_packed[C(x, y)]);
        public Piece? this[Vector2Int coordinate] => this[coordinate.x, coordinate.y];

        public Board(IEnumerable<(int, int, Piece)> occupiedSpaces)
        {
            pieces_packed = new byte[DIMENSION * DIMENSION];

            foreach ((int x, int y, Piece piece) in occupiedSpaces)
                pieces_packed[x + y * DIMENSION] = Piece.Pack(piece);
        }

        public Board(string layout)
        {
            pieces_packed = new byte[DIMENSION * DIMENSION];

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
                .Split("\n")
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

                    pieces_packed[C(x, y)] = Piece.Pack(new Piece(form, owner));
                }
            }
        }

        internal Board(byte[] pieces)
        {
            this.pieces_packed = pieces;
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
            return pieces_packed.SequenceEqual(other.pieces_packed);
        }

        public override int GetHashCode()
        {
            return (
                pieces_packed[0],
                pieces_packed[1],
                pieces_packed[3],
                pieces_packed[7],
                pieces_packed[13],
                pieces_packed[17]
            ).GetHashCode();
        }

        public Board Clone()
        {
            return new Board((byte[])pieces_packed.Clone());
        }

        internal static void ApplyActionComponent(byte[] array, ActionComponent component)
        {
            // ReSharper disable once PossibleInvalidOperationException
            Piece original = Piece.Unpack(array[C(component.Origin)]).Value;

            var newPieceAtOrigin =
                original.Movement.Interaction == Interaction.Swap
                    ? Piece.Unpack(array[C(component.Destination)])
                    : null;

            var newPieceAtDestination = new Piece(component.Transformation, original.Owner);

            array[C(component.Origin)] = Piece.Pack(newPieceAtOrigin);
            array[C(component.Destination)] = Piece.Pack(newPieceAtDestination);
        }

        internal Board ApplyAction(GameAction action)
        {
            var clone = (byte[])pieces_packed.Clone();

            ApplyActionComponent(clone, action.First);
            if (action.Second is { } second)
                ApplyActionComponent(clone, second);

            return new Board(clone);
        }

        private static int C(int x, int y) => x + y * DIMENSION;

        private static int C(Vector2Int coordinate) => C(coordinate.x, coordinate.y);
    }
}
