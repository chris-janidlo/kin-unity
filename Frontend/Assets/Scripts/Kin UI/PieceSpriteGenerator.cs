using Core_Rules;
using crass;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kin_UI
{
    [CreateAssetMenu(
        menuName = "LDJ50/Kin Sprite Generator",
        fileName = "newSpriteGenerator.asset"
    )]
    public class PieceSpriteGenerator : ScriptableObject
    {
        [FormerlySerializedAs("FormSprites")]
        public EnumMap<Form, Sprite> formSprites;

        [FormerlySerializedAs("PlayerColors")]
        public EnumMap<Player, Color> playerColors;

        public (Sprite, Color) GetSprite(Piece? potentialPiece)
        {
            return potentialPiece is { } piece
                ? (formSprites[piece.Form], playerColors[piece.Owner])
                : (null, Color.clear);
        }

        public Sprite GetGraphic(Form form)
        {
            return formSprites[form];
        }
    }
}
