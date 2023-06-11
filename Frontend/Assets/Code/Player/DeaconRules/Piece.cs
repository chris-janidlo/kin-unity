using System;
using Unity.Mathematics;
using Unity.Collections;
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

        private static readonly NativeArray<int2> _CARDINAL_DIRECTIONS =
            new(
                new[] { new int2(0, 1), new int2(0, -1), new int2(1, 0), new int2(-1, 0), },
                Allocator.Persistent
            );

        private static readonly NativeArray<int2> _DIAGONAL_DIRECTIONS =
            new(
                new[] { new int2(1, 1), new int2(1, -1), new int2(-1, 1), new int2(-1, -1), },
                Allocator.Persistent
            );

        private static readonly NativeArray<int2> _ALL_DIRECTIONS =
            new(
                new[]
                {
                    new int2(0, 1),
                    new int2(0, -1),
                    new int2(1, 0),
                    new int2(1, 1),
                    new int2(1, -1),
                    new int2(-1, 0),
                    new int2(-1, 1),
                    new int2(-1, -1),
                },
                Allocator.Persistent
            );

        private static readonly MovementData _CAPTAIN_DATA = new MovementData(
            5,
            _ALL_DIRECTIONS,
            Interaction.Capture,
            new NativeArray<Form>(new[] { Form.Scientist }, Allocator.Persistent)
        );

        private static readonly MovementData _ENGINEER_DATA = new MovementData(
            2,
            _DIAGONAL_DIRECTIONS,
            Interaction.Swap,
            new NativeArray<Form>(new[] { Form.Pilot, Form.Priest }, Allocator.Persistent)
        );

        private static readonly MovementData _PILOT_DATA = new MovementData(
            2,
            _DIAGONAL_DIRECTIONS,
            Interaction.Capture,
            new NativeArray<Form>(
                new[] { Form.Engineer, Form.Priest, Form.Captain },
                Allocator.Persistent
            )
        );

        private static readonly MovementData _PRIEST_DATA = new MovementData(
            3,
            _CARDINAL_DIRECTIONS,
            Interaction.Swap,
            new NativeArray<Form>(new[] { Form.Robot, Form.Engineer }, Allocator.Persistent)
        );

        private static readonly MovementData _ROBOT_DATA = new MovementData(
            2,
            _CARDINAL_DIRECTIONS,
            Interaction.Capture,
            new NativeArray<Form>(
                new[] { Form.Engineer, Form.Priest, Form.Captain },
                Allocator.Persistent
            )
        );

        private static readonly MovementData _SCIENTIST_DATA = new MovementData(
            1,
            _ALL_DIRECTIONS,
            Interaction.None,
            new NativeArray<Form>(new[] { Form.Engineer, Form.Priest }, Allocator.Persistent)
        );

        public MovementData Movement =>
            Form switch
            {
                Form.Captain => _CAPTAIN_DATA,
                Form.Engineer => _ENGINEER_DATA,
                Form.Pilot => _PILOT_DATA,
                Form.Priest => _PRIEST_DATA,
                Form.Robot => _ROBOT_DATA,
                Form.Scientist => _SCIENTIST_DATA,
                _ => throw new ArgumentOutOfRangeException()
            };

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
        public readonly NativeArray<int2> Directions;
        public readonly Interaction Interaction;
        public readonly NativeArray<Form> Transformations;

        public MovementData(
            int range,
            NativeArray<int2> directions,
            Interaction interaction,
            NativeArray<Form> transformations
        )
        {
            Range = range;
            Directions = directions;
            Interaction = interaction;
            Transformations = transformations;
        }
    }
}
