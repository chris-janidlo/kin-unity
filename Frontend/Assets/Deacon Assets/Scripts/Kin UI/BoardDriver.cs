using System;
using System.Collections.Generic;
using System.Linq;
using Core_Rules;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Decisions;

namespace Kin_UI
{
    public class BoardDriver : MonoBehaviour
    {
        [FormerlySerializedAs("Cells")]
        public List<Cell> cells;

        [FormerlySerializedAs("ResetButton")]
        public Button resetButton;

        [FormerlySerializedAs("ConfirmButton")]
        public Button confirmButton;

        [FormerlySerializedAs("FormButtons")]
        public FormPopup formButtons;

        [FormerlySerializedAs("PlayerDecider")]
        public PlayerDecider playerDecider;

        private Vector2Int _currentlyMoving,
            _currentTarget;
        private GameState _masterState,
            _workingState;
        private List<char> _pieceIdsMoved;

        private Dictionary<Vector2Int, Cell> _positionMap;

        private void Awake()
        {
            _positionMap = new Dictionary<Vector2Int, Cell>();
            _pieceIdsMoved = new List<char>();

            foreach (var cell in cells)
            {
                if (_positionMap.TryGetValue(cell.coordinates, out var existingCell))
                    throw new InvalidOperationException(
                        $"cannot add Cell {cell.name} to {name} because"
                            + $" there is already a Cell ({existingCell.name}) at position {cell.coordinates}"
                    );

                _positionMap[cell.coordinates] = cell;
            }

            resetButton.onClick.AddListener(OnResetButtonClicked);
            formButtons.RegisterCallback(PieceTransformationCallback);
            ResetButtons(ButtonType.NonGrid);
        }

        public void OnPlayerDecisionNeeded(GameState gameState)
        {
            _workingState = _masterState = gameState;
            _pieceIdsMoved.Clear();

            ResetButtons();
            SetupMoveablePieces();

            DrawBoard(_masterState);
        }

        public void OnResetButtonClicked()
        {
            _workingState = _masterState;
            _pieceIdsMoved.Clear();

            ResetButtons();
            SetupMoveablePieces();

            DrawBoard(_masterState);
        }

        public void OnConfirmButtonClicked()
        {
            _masterState = _workingState;
            _masterState.FlipPlayer();

            ResetButtons();

            playerDecider.PlayerDecisionMade(_masterState);
        }

        private void ResetButtons(ButtonType types = ButtonType.All)
        {
            if (types.HasFlag(ButtonType.Reset))
                resetButton.interactable = false;
            if (types.HasFlag(ButtonType.Confirm))
                confirmButton.interactable = false;

            foreach (var cell in cells)
                if (
                    (
                        types.HasFlag(ButtonType.MoveablePiece)
                        && cell.GetClickCallback() == MoveablePieceClickCallback
                    )
                    || (
                        types.HasFlag(ButtonType.PieceMovementTarget)
                        && cell.GetClickCallback() == PieceMovementTargetClickCallback
                    )
                )
                    cell.RegisterClickCallback(null);

            if (types.HasFlag(ButtonType.PieceTransformation))
                formButtons.TearDownPopup();
        }

        private void SetupMoveablePieces(Vector2Int? exceptPosition = null)
        {
            foreach (var playerPiece in _workingState.PiecesOwnedByCurrentPlayer())
            {
                if (
                    playerPiece.Position == exceptPosition
                    || _pieceIdsMoved.Contains(playerPiece.ID)
                )
                    continue;

                _positionMap[playerPiece.Position].RegisterClickCallback(
                    MoveablePieceClickCallback
                );
            }
        }

        private void DrawBoard(GameState state)
        {
            cells.ForEach(c => c.UpdateImage(state));
        }

        private void MoveablePieceClickCallback(Cell cell)
        {
            ResetButtons(ButtonType.AllGrid);
            SetupMoveablePieces(cell.coordinates);

            foreach (var position in _workingState.LegalPositionsForPieceAt(cell.coordinates))
                _positionMap[position].RegisterClickCallback(PieceMovementTargetClickCallback);

            _currentlyMoving = cell.coordinates;
        }

        private void PieceMovementTargetClickCallback(Cell cell)
        {
            ResetButtons(ButtonType.AllGrid);
            SetupMoveablePieces();

            formButtons.SetUpPopup(
                cell,
                _workingState.LegalFormTransitionsForPieceAt(_currentlyMoving)
            );

            _currentTarget = cell.coordinates;
        }

        private void PieceTransformationCallback(Form form)
        {
            ResetButtons(ButtonType.AllGrid);

            var pieceMoved = _workingState.Board.GetPiece(_currentlyMoving).Value;
            var resultingPiece = new Piece(pieceMoved.ID, pieceMoved.Owner, form, _currentTarget);

            _workingState = GameState.ApplyMove(_workingState, pieceMoved, resultingPiece, false);
            resetButton.interactable = true;

            _pieceIdsMoved.Add(pieceMoved.ID);
            SetupMoveablePieces();

            if (_pieceIdsMoved.Count == _workingState.PiecesOwnedByCurrentPlayer().Count())
                confirmButton.interactable = true;

            DrawBoard(_workingState);
        }

        [Flags]
        private enum ButtonType
        {
            [UsedImplicitly]
            None = 0,

            Reset = 1 << 0,
            Confirm = 1 << 1,
            NonGrid = Reset | Confirm,

            MoveablePiece = 1 << 2,
            PieceMovementTarget = 1 << 3,
            PieceTransformation = 1 << 4,
            AllGrid = MoveablePiece | PieceMovementTarget | PieceTransformation,

            All = NonGrid | AllGrid
        }
    }
}
