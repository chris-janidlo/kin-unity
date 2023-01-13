using System.Threading;
using Core_Rules;
using Cysharp.Threading.Tasks;
using UnityAtoms.LDJ50;
using UnityEngine;
using UnityEngine.Serialization;

namespace Decisions
{
    [CreateAssetMenu(menuName = "LDJ50/Deciders/Player", fileName = "newPlayerDecider.asset")]
    public class PlayerDecider : ADecider
    {
        [FormerlySerializedAs("PlayerDecisionNeeded")]
        public GameStateEvent playerDecisionNeeded;

        private UniTaskCompletionSource<GameState> _currentTcs;

        // TODO: use cancellation token
        public override UniTask<GameState> DecideMove(GameState currentState, CancellationToken token)
        {
            Deciding = true;
            playerDecisionNeeded.Raise(currentState);

            _currentTcs = new UniTaskCompletionSource<GameState>();
            return _currentTcs.Task;
        }

        public void PlayerDecisionMade(GameState decision)
        {
            Deciding = false;
            _currentTcs.TrySetResult(decision);
        }
    }
}
