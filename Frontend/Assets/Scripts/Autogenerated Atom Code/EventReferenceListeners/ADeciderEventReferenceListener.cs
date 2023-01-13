using UnityEngine;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event Reference Listener of type `ADecider`. Inherits from `AtomEventReferenceListener&lt;ADecider, ADeciderEvent, ADeciderEventReference, ADeciderUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/ADecider Event Reference Listener")]
    public sealed class ADeciderEventReferenceListener : AtomEventReferenceListener<
        ADecider,
        ADeciderEvent,
        ADeciderEventReference,
        ADeciderUnityEvent>
    { }
}
