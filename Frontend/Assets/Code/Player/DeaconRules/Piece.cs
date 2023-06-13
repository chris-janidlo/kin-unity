using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Player.DeaconRules
{
    public readonly struct Piece
    {
        public readonly Form Form;
        public readonly Player Owner;

        public Piece(Form form, Player owner)
        {
            Form = form;
            Owner = owner;
        }

        private static readonly Vector2Int[] _CARDINAL_DIRECTIONS = new[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
        };

        private static readonly Vector2Int[] _DIAGONAL_DIRECTIONS = new[]
        {
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
        };

        private static readonly Vector2Int[] _ALL_DIRECTIONS = new[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
        };

        private static readonly Dictionary<Form, MovementData> _MAP = new Dictionary<
            Form,
            MovementData
        >
        {
            {
                Form.Captain,
                new MovementData(5, _ALL_DIRECTIONS, Interaction.Capture, new[] { Form.Scientist })
            },
            {
                Form.Engineer,
                new MovementData(
                    2,
                    _DIAGONAL_DIRECTIONS,
                    Interaction.Swap,
                    new[] { Form.Pilot, Form.Priest }
                )
            },
            {
                Form.Pilot,
                new MovementData(
                    2,
                    _DIAGONAL_DIRECTIONS,
                    Interaction.Capture,
                    new[] { Form.Engineer, Form.Priest, Form.Captain }
                )
            },
            {
                Form.Priest,
                new MovementData(
                    3,
                    _CARDINAL_DIRECTIONS,
                    Interaction.Swap,
                    new[] { Form.Robot, Form.Engineer }
                )
            },
            {
                Form.Robot,
                new MovementData(
                    2,
                    _CARDINAL_DIRECTIONS,
                    Interaction.Capture,
                    new[] { Form.Engineer, Form.Priest, Form.Captain }
                )
            },
            {
                Form.Scientist,
                new MovementData(
                    1,
                    _ALL_DIRECTIONS,
                    Interaction.None,
                    new[] { Form.Engineer, Form.Priest }
                )
            }
        };

        public MovementData Movement => _MAP[Form];

        public override string ToString()
        {
            return ToChar().ToString();
        }

        public char ToChar()
        {
            char c = Form switch
            {
                Form.Captain => 'c',
                Form.Engineer => 'e',
                Form.Pilot => 'i',
                Form.Priest => 'p',
                Form.Robot => 'r',
                Form.Scientist => 's',
                _ => throw new ArgumentOutOfRangeException(Form.ToString())
            };

            if (Owner == Player.Blue)
                c = char.ToUpper(c);

            return c;
        }

        public static Piece? Unpack(byte value)
        {
            if (value == 0)
                return null;

            int playerBit = value & 1;
            int formBits = value >> 1;

            Player player = playerBit == 1 ? Player.Red : Player.Blue;
            var form = (Form)(formBits - 1);

            return new Piece(form, player);
        }

        public static byte Pack(Piece? value)
        {
            if (value == null)
                return 0;

            Piece actual = value.Value;

            int playerBit = actual.Owner switch
            {
                Player.Red => 1,
                Player.Blue => 0,
                _ => throw new ArgumentOutOfRangeException()
            };
            int formBits = ((int)actual.Form) + 1;

            return (byte)((formBits << 1) + playerBit);
        }
    }

    public enum Form
    {
        Captain = 0,
        Engineer = 1,
        Pilot = 2,
        Priest = 3,
        Robot = 4,
        Scientist = 5,
    }

    public enum Interaction
    {
        None,
        Capture,
        Swap,
    }

    public readonly struct MovementData
    {
        public readonly int Range;
        public readonly Vector2Int[] Directions;
        public readonly Interaction Interaction;
        public readonly Form[] Transformations;

        public MovementData(
            int range,
            Vector2Int[] directions,
            Interaction interaction,
            Form[] transformations
        )
        {
            Range = range;
            Directions = directions;
            Interaction = interaction;
            Transformations = transformations;
        }
    }
}
