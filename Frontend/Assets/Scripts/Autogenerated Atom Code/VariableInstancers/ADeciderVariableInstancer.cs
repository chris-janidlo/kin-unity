using UnityEngine;
using UnityAtoms.BaseAtoms;
using Decisions;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Variable Instancer of type `ADecider`. Inherits from `AtomVariableInstancer&lt;ADeciderVariable, ADeciderPair, ADecider, ADeciderEvent, ADeciderPairEvent, ADeciderADeciderFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-hotpink")]
    [AddComponentMenu("Unity Atoms/Variable Instancers/ADecider Variable Instancer")]
    public class ADeciderVariableInstancer : AtomVariableInstancer<
        ADeciderVariable,
        ADeciderPair,
        ADecider,
        ADeciderEvent,
        ADeciderPairEvent,
        ADeciderADeciderFunction>
    { }
}
