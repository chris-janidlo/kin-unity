using UnityEngine;
using Core_Rules;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event Instancer of type `GameState`. Inherits from `AtomEventInstancer&lt;GameState, GameStateEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/GameState Event Instancer")]
    public class GameStateEventInstancer : AtomEventInstancer<GameState, GameStateEvent> { }
}
