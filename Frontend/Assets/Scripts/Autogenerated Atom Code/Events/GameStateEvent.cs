using UnityEngine;
using Core_Rules;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event of type `GameState`. Inherits from `AtomEvent&lt;GameState&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/GameState", fileName = "GameStateEvent")]
    public sealed class GameStateEvent : AtomEvent<GameState> { }
}
