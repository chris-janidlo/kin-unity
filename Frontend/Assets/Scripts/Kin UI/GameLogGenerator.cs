using Core_Rules;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kin_UI
{
    [CreateAssetMenu(menuName = "LDJ50/Game Log Generator", fileName = "newGameLogGenerator.asset")]
    public class GameLogGenerator : ScriptableObject
    {
        [FormerlySerializedAs("PieceStateFormat")]
        [Tooltip("Expects the following placeholders: {0}, which will be replaced with the piece's position, " +
                 "and {1}, which will be replaced with the name of its form.")]
        public string pieceStateFormat;

        [FormerlySerializedAs("PieceMoveLogFormat")]
        [Tooltip("Expects the following placeholders: {0}, which will be replaced with the piece's initial state, " +
                 "and {1}, which will be replaced with its final state.")]
        public string pieceMoveLogFormat;

        public string GetLogEntryForAction(GameState oldState, GameState newState)
        {
            var result = $"{oldState.CurrentPlayer}:";

            foreach (var initialPieceState in oldState.PiecesOwnedByCurrentPlayer())
            {
                var finalPieceState = newState.GetPieceById(initialPieceState.ID).Value;
                result += "\n";
                result += string.Format(pieceMoveLogFormat, PieceState(initialPieceState), PieceState(finalPieceState));
            }

            return result;
        }

        private string PieceState(Piece piece)
        {
            var x = (char)(piece.Position.x + 'a');
            var y = (piece.Position.y + 1).ToString();

            var position = x + y;
            var formName = piece.Form.ToString().ToLower();

            return string.Format(pieceStateFormat, position, formName);
        }
    }
}
