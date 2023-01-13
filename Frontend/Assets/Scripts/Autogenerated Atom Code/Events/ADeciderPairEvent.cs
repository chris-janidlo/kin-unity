using UnityEngine;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event of type `ADeciderPair`. Inherits from `AtomEvent&lt;ADeciderPair&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/ADeciderPair", fileName = "ADeciderPairEvent")]
    public sealed class ADeciderPairEvent : AtomEvent<ADeciderPair> { }
}
