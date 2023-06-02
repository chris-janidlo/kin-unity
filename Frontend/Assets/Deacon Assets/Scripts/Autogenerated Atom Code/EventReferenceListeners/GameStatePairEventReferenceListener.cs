using UnityEngine;
using Core_Rules;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event Reference Listener of type `GameStatePair`. Inherits from `AtomEventReferenceListener&lt;GameStatePair, GameStatePairEvent, GameStatePairEventReference, GameStatePairUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/GameStatePair Event Reference Listener")]
    public sealed class GameStatePairEventReferenceListener
        : AtomEventReferenceListener<
            GameStatePair,
            GameStatePairEvent,
            GameStatePairEventReference,
            GameStatePairUnityEvent
        > { }
}
