using UnityEngine;
using Decisions;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Value List of type `ADecider`. Inherits from `AtomValueList&lt;ADecider, ADeciderEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-piglet")]
    [CreateAssetMenu(menuName = "Unity Atoms/Value Lists/ADecider", fileName = "ADeciderValueList")]
    public sealed class ADeciderValueList : AtomValueList<ADecider, ADeciderEvent> { }
}
