using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Core_Rules
{
    public readonly struct Board
    {
        public const int SideLength = 5;

        private readonly Piece?[,] _positions;
        private readonly int _sideLength;

        public Board(int sideLength)
        {
            _sideLength = sideLength;
            _positions = new Piece?[sideLength, sideLength];
        }

        public static Board CreateBoard()
        {
            return new Board(SideLength);
        }

        public static Board DeepClone(Board other)
        {
            var result = CreateBoard();
            for (var x = 0; x < SideLength; x++)
                for (var y = 0; y < SideLength; y++)
                    if (other._positions[x, y] is { } piece)
                        result._positions[x, y] = piece.Clone();
            return result;
        }

        public bool InBounds(Vector2Int position)
        {
            return position.x >= 0
                && position.x < _sideLength
                && position.y >= 0
                && position.y < _sideLength;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (_positions == null)
                    return 0;
                var hash = 17;
                foreach (var element in _positions)
                    hash = hash * 31 + element.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            for (var x = 0; x < SideLength; x++)
            {
                for (var y = 0; y < SideLength; y++)
                {
                    result.Append(_positions[x, y]?.ToChar() ?? '_');
                    result.Append(' ');
                }

                result.Append('\n');
            }

            return result.ToString();
        }

        public Piece? GetPiece(Vector2Int position)
        {
            return _positions[position.x, position.y];
        }

        public void SetPiece(Piece piece, Vector2Int position)
        {
            _positions[position.x, position.y] = piece;
        }

        public void RemovePiece(Vector2Int position)
        {
            _positions[position.x, position.y] = null;
        }

        public IEnumerable<Piece> GetPiecesByOwner(Player owner)
        {
            foreach (var piece in _positions)
                if (piece?.Owner == owner)
                    yield return piece.Value;
        }

        public bool IsLossForPlayer(Player player)
        {
            foreach (var piece in _positions)
                if (piece?.Owner == player)
                    return false;
            return true;
        }

        public Piece? GetPieceById(char id)
        {
            return _positions.Cast<Piece?>().FirstOrDefault(p => p?.ID == id);
        }
    }
}
