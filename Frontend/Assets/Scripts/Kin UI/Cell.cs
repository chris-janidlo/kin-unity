using Core_Rules;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Kin_UI
{
    public class Cell : MonoBehaviour
    {
        public delegate void ClickCallback(Cell cell);

        [FormerlySerializedAs("Coordinates")]
        public Vector2Int coordinates;

        [FormerlySerializedAs("PieceImage")]
        public Image pieceImage;

        [FormerlySerializedAs("Button")]
        public Button button;

        [FormerlySerializedAs("PieceSpriteGenerator")]
        public PieceSpriteGenerator pieceSpriteGenerator;

        private ClickCallback _currentCallback;

        public void RegisterClickCallback(ClickCallback callback)
        {
            _currentCallback = callback;
            button.interactable = _currentCallback != null;
        }

        public ClickCallback GetClickCallback()
        {
            return _currentCallback;
        }

        public void OnClick()
        {
            _currentCallback?.Invoke(this);
        }

        public void UpdateImage(GameState state)
        {
            (pieceImage.sprite, pieceImage.color) = pieceSpriteGenerator.GetSprite(
                state.Board.GetPiece(coordinates)
            );
        }
    }
}
