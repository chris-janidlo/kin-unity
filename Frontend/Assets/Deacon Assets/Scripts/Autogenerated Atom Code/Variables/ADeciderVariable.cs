using UnityEngine;
using Decisions;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Variable of type `ADecider`. Inherits from `EquatableAtomVariable&lt;ADecider, ADeciderPair, ADeciderEvent, ADeciderPairEvent, ADeciderADeciderFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/ADecider", fileName = "ADeciderVariable")]
    public sealed class ADeciderVariable
        : EquatableAtomVariable<
            ADecider,
            ADeciderPair,
            ADeciderEvent,
            ADeciderPairEvent,
            ADeciderADeciderFunction
        > { }
}
