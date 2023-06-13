using UnityEngine;
using UnityAtoms.BaseAtoms;
using Decisions;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Set variable value Action of type `ADecider`. Inherits from `SetVariableValue&lt;ADecider, ADeciderPair, ADeciderVariable, ADeciderConstant, ADeciderReference, ADeciderEvent, ADeciderPairEvent, ADeciderVariableInstancer&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-purple")]
    [CreateAssetMenu(
        menuName = "Unity Atoms/Actions/Set Variable Value/ADecider",
        fileName = "SetADeciderVariableValue"
    )]
    public sealed class SetADeciderVariableValue
        : SetVariableValue<
            ADecider,
            ADeciderPair,
            ADeciderVariable,
            ADeciderConstant,
            ADeciderReference,
            ADeciderEvent,
            ADeciderPairEvent,
            ADeciderADeciderFunction,
            ADeciderVariableInstancer
        > { }
}
