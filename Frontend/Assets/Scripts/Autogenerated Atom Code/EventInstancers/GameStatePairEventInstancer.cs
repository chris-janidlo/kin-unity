using UnityEngine;
using Core_Rules;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event Instancer of type `GameStatePair`. Inherits from `AtomEventInstancer&lt;GameStatePair, GameStatePairEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/GameStatePair Event Instancer")]
    public class GameStatePairEventInstancer : AtomEventInstancer<GameStatePair, GameStatePairEvent> { }
}
