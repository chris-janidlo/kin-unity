using System.Linq;
using System.Threading;
using Core_Rules;
using crass;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Decisions
{
    [CreateAssetMenu(menuName = "LDJ50/Deciders/Random", fileName = "newRandomDecider.asset")]
    public class RandomDecider : ADecider
    {
        public override UniTask<GameState> DecideMove(
            GameState currentState,
            CancellationToken token
        )
        {
            var movePool = currentState.LegalFutureStates().ToList();
            return UniTask.FromResult(movePool.PickRandom()).AttachExternalCancellation(token);
        }
    }
}