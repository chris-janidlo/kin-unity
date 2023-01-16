using System;
using System.Collections.Generic;
using System.Linq;
using Core_Rules;
using crass;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Kin_UI
{
    public class HelpPagePieceEntry : MonoBehaviour
    {
        [FormerlySerializedAs("Form")]
        public Form form;

        [FormerlySerializedAs("ActiveBoardCellColor")]
        public Color activeBoardCellColor;

        [FormerlySerializedAs("InteractionDescriptions")]
        public EnumMap<PieceInteraction, string> interactionDescriptions;

        [FormerlySerializedAs("PieceImages")]
        public List<Image> pieceImages;

        [FormerlySerializedAs("Name")]
        public TextMeshProUGUI pieceName;

        [FormerlySerializedAs("BoardCells")]
        public List<Image> boardCells;

        [FormerlySerializedAs("FormTransformationImages")]
        public EnumMap<Form, Image> formTransformationImages;

        [FormerlySerializedAs("CellInteractionText")]
        public TextMeshProUGUI cellInteractionText;

        [FormerlySerializedAs("PieceSpriteGenerator")]
        public PieceSpriteGenerator pieceSpriteGenerator;

        private void Start()
        {
            DrawSummary();
            DrawMovement();
            DrawTransformations();
            DrawInteraction();
        }

        private void DrawSummary()
        {
            foreach (var image in pieceImages)
                image.sprite = pieceSpriteGenerator.GetGraphic(form);

            pieceName.text = form.ToString();
        }

        private void DrawMovement()
        {
            var sideLength = (int)Math.Sqrt(boardCells.Count);
            var fakeBoard = new Board(sideLength);
            var boardCenter = new Vector2Int(sideLength / 2, sideLength / 2);

            var validPositions = form.GetLegalBoardPositions(boardCenter, fakeBoard).ToList();

            for (var i = 0; i < boardCells.Count; i++)
            {
                var coordinate = new Vector2Int(i % sideLength, i / sideLength);
                if (coordinate == boardCenter)
                    continue;

                if (validPositions.Contains(coordinate))
                    boardCells[i].color = activeBoardCellColor;
            }
        }

        private void DrawTransformations()
        {
            var formTransformations = form.GetFormTransitions().ToList();

            foreach (var value in EnumUtil.AllValues<Form>())
                formTransformationImages[value].gameObject.SetActive(
                    formTransformations.Contains(value)
                );
        }

        private void DrawInteraction()
        {
            cellInteractionText.text = interactionDescriptions[form.GetInteraction()];
        }
    }
}
