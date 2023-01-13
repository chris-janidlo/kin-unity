using UnityEngine;
using Decisions;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event Reference Listener of type `ADeciderPair`. Inherits from `AtomEventReferenceListener&lt;ADeciderPair, ADeciderPairEvent, ADeciderPairEventReference, ADeciderPairUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/ADeciderPair Event Reference Listener")]
    public sealed class ADeciderPairEventReferenceListener : AtomEventReferenceListener<
        ADeciderPair,
        ADeciderPairEvent,
        ADeciderPairEventReference,
        ADeciderPairUnityEvent>
    { }
}
