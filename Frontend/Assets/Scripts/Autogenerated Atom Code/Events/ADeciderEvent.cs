using UnityEngine;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event of type `ADecider`. Inherits from `AtomEvent&lt;ADecider&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/ADecider", fileName = "ADeciderEvent")]
    public sealed class ADeciderEvent : AtomEvent<ADecider> { }
}
