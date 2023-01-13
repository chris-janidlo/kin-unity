using System.Linq;
using System.Threading;
using Core_Rules;
using Cysharp.Threading.Tasks;
using UnityAtoms.LDJ50;
using UnityEngine;
using UnityEngine.Serialization;

public class GameLoop : MonoBehaviour
{
    [FormerlySerializedAs("BlueDecider")] public ADeciderVariable blueDecider;
    [FormerlySerializedAs("RedDecider")] public ADeciderVariable redDecider;
    [FormerlySerializedAs("DecisionMade")] public GameStatePairEvent decisionMade;

    [FormerlySerializedAs("GameOverOverlay")]
    public GameOverOverlay gameOverOverlay;

    private GameState _gameState;

    private void Start()
    {
        StartCoroutine(PlayOutGame(this.GetCancellationTokenOnDestroy()).ToCoroutine());
    }

    private async UniTask PlayOutGame(CancellationToken token)
    {
        _gameState = GameState.InitialGameState();

        while (!_gameState.IsLossState && _gameState.LegalFutureStates().Any())
        {
            var nextDecider = _gameState.CurrentPlayer == Player.Blue
                ? blueDecider.Value
                : redDecider.Value;
            var newState = await nextDecider.DecideMove(_gameState, token);

            decisionMade.Raise(new GameStatePair { Item1 = _gameState, Item2 = newState });

            _gameState = newState;
        }

        gameOverOverlay.ShowOverlay(_gameState.CurrentPlayer);
    }
}
