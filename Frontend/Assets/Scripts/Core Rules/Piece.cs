using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core_Rules
{
    public struct Piece
    {
        public readonly char ID;
        public readonly Player Owner;

        public readonly Form Form;
        public Vector2Int Position;

        public static bool operator ==(Piece lhs, Piece rhs)
        {
            return lhs.Owner == rhs.Owner &&
                   lhs.Form == rhs.Form &&
                   lhs.Position == rhs.Position;
        }

        public static bool operator !=(Piece lhs, Piece rhs)
        {
            return !(lhs == rhs);
        }

        private bool Equals(Piece other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Piece other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Owner, Form, Position).GetHashCode();
            // deliberately exclude ID from the hashcode computation because we use the hashcode to calculate value
            // equality, and we want to treat two pieces that are identical except for their ID as equal when comparing
            // board layouts
        }

        public Piece(char id, Player owner, Form form, Vector2Int position)
        {
            ID = id;
            Owner = owner;
            Form = form;
            Position = position;
        }

        public IEnumerable<Piece> LegalMoves(Board board)
        {
            foreach (var legalPosition in Form.GetLegalBoardPositions(Position, board))
                foreach (var legalForm in Form.GetFormTransitions())
                    yield return new Piece(ID, Owner, legalForm, legalPosition);
        }

        public char ToChar()
        {
            var c = Form switch
            {
                Form.Captain => 'c',
                Form.Engineer => 'e',
                Form.Pilot => 'i',
                Form.Priest => 'p',
                Form.Robot => 'r',
                Form.Scientist => 's',
                _ => throw new ArgumentOutOfRangeException(Form.ToString())
            };

            return Owner switch
            {
                Player.Blue => char.ToLower(c),
                Player.Red => char.ToUpper(c),
                _ => throw new ArgumentOutOfRangeException(Form.ToString())
            };
        }

        public Piece Clone()
        {
            return new Piece(ID, Owner, Form, Position);
        }
    }
}
