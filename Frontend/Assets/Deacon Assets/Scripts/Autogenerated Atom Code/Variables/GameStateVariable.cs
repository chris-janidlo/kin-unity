using UnityEngine;
using Core_Rules;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Variable of type `GameState`. Inherits from `EquatableAtomVariable&lt;GameState, GameStatePair, GameStateEvent, GameStatePairEvent, GameStateGameStateFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/GameState", fileName = "GameStateVariable")]
    public sealed class GameStateVariable
        : EquatableAtomVariable<
            GameState,
            GameStatePair,
            GameStateEvent,
            GameStatePairEvent,
            GameStateGameStateFunction
        > { }
}
