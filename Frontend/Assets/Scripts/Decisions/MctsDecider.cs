using System.Threading;
using System.Threading.Tasks;
using Core_Rules;
using Cysharp.Threading.Tasks;
using MCTS;
using UnityEngine;
using UnityEngine.Serialization;

namespace Decisions
{
    [CreateAssetMenu(menuName = "LDJ50/Deciders/MCTS", fileName = "newMctsDecider.asset")]
    public class MctsDecider : AIDecider
    {
        [FormerlySerializedAs("SearchParameters")]
        public MctsParameters searchParameters;

        private MctsSearcher _searcher;

        public override async UniTask<GameState> DecideMove(GameState currentState, CancellationToken token)
        {
            Deciding = true;
            Progress = 0;

            _searcher ??= new MctsSearcher(currentState.CurrentPlayer, searchParameters);

            var result = await Task.Run(() => _searcher.Search(currentState, p => Progress = p), token);
            Deciding = false;

            return result;
        }
    }
}
