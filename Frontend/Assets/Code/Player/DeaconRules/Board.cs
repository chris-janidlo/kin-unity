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

        public struct Array
        {
            private byte zero;
            private byte one;
            private byte two;
            private byte three;
            private byte four;
            private byte five;
            private byte six;
            private byte seven;
            private byte eight;
            private byte nine;
            private byte ten;
            private byte eleven;
            private byte twelve;
            private byte thirteen;
            private byte fourteen;
            private byte fifteen;
            private byte sixteen;
            private byte seventeen;
            private byte eighteen;
            private byte nineteen;
            private byte twenty;
            private byte twentyOne;
            private byte twentyTwo;
            private byte twentyThree;
            private byte twentyFour;

            public int Length => DIMENSION * DIMENSION;

            public byte this[int i]
            {
                get =>
                    i switch
                    {
                        0 => zero,
                        1 => one,
                        2 => two,
                        3 => three,
                        4 => four,
                        5 => five,
                        6 => six,
                        7 => seven,
                        8 => eight,
                        9 => nine,
                        10 => ten,
                        11 => eleven,
                        12 => twelve,
                        13 => thirteen,
                        14 => fourteen,
                        15 => fifteen,
                        16 => sixteen,
                        17 => seventeen,
                        18 => eighteen,
                        19 => nineteen,
                        20 => twenty,
                        21 => twentyOne,
                        22 => twentyTwo,
                        23 => twentyThree,
                        24 => twentyFour,
                        _
                            => throw new ArgumentOutOfRangeException(
                                nameof(i),
                                i,
                                $"{nameof(Board)} {nameof(Array)}s have {Length} members"
                            )
                    };
                set
                {
                    switch (i)
                    {
                        case 0:
                            zero = value;
                            break;
                        case 1:
                            one = value;
                            break;
                        case 2:
                            two = value;
                            break;
                        case 3:
                            three = value;
                            break;
                        case 4:
                            four = value;
                            break;
                        case 5:
                            five = value;
                            break;
                        case 6:
                            six = value;
                            break;
                        case 7:
                            seven = value;
                            break;
                        case 8:
                            eight = value;
                            break;
                        case 9:
                            nine = value;
                            break;
                        case 10:
                            ten = value;
                            break;
                        case 11:
                            eleven = value;
                            break;
                        case 12:
                            twelve = value;
                            break;
                        case 13:
                            thirteen = value;
                            break;
                        case 14:
                            fourteen = value;
                            break;
                        case 15:
                            fifteen = value;
                            break;
                        case 16:
                            sixteen = value;
                            break;
                        case 17:
                            seventeen = value;
                            break;
                        case 18:
                            eighteen = value;
                            break;
                        case 19:
                            nineteen = value;
                            break;
                        case 20:
                            twenty = value;
                            break;
                        case 21:
                            twentyOne = value;
                            break;
                        case 22:
                            twentyTwo = value;
                            break;
                        case 23:
                            twentyThree = value;
                            break;
                        case 24:
                            twentyFour = value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(i),
                                i,
                                $"{nameof(Board)} {nameof(Array)}s have {Length} members"
                            );
                    }
                }
            }
        }

        internal readonly Array pieces_packed;

        // [0, 0] is top left of board
        // increasing x coordinates go toward the right
        // increasing y coordinates go downward
        public Piece? this[int x, int y] => Piece.Unpack(pieces_packed[C(x, y)]);
        public Piece? this[Vector2Int coordinate] => this[coordinate.x, coordinate.y];

        public Board(IEnumerable<(int, int, Piece)> occupiedSpaces)
        {
            pieces_packed = new();

            foreach ((int x, int y, Piece piece) in occupiedSpaces)
                pieces_packed[x + y * DIMENSION] = Piece.Pack(piece);
        }

        public Board(string layout)
        {
            pieces_packed = new();

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

        internal Board(Array pieces)
        {
            pieces_packed = pieces;
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
            for (var i = 0; i < pieces_packed.Length; i++)
            {
                if (pieces_packed[i] != other.pieces_packed[i])
                    return false;
            }

            return true;
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

        internal static void ApplyActionComponent(ref Array array, ActionComponent component)
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
            Array clone = pieces_packed; // TODO: verify that this works

            ApplyActionComponent(ref clone, action.First);
            if (action.Second is { } second)
                ApplyActionComponent(ref clone, second);

            return new Board(clone);
        }

        private static int C(int x, int y) => x + y * DIMENSION;

        private static int C(Vector2Int coordinate) => C(coordinate.x, coordinate.y);
    }
}
