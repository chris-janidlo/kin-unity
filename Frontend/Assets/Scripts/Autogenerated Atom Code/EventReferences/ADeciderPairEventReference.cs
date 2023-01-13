using System;
using Decisions;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event Reference of type `ADeciderPair`. Inherits from `AtomEventReference&lt;ADeciderPair, ADeciderVariable, ADeciderPairEvent, ADeciderVariableInstancer, ADeciderPairEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class ADeciderPairEventReference : AtomEventReference<
        ADeciderPair,
        ADeciderVariable,
        ADeciderPairEvent,
        ADeciderVariableInstancer,
        ADeciderPairEventInstancer>, IGetEvent
    { }
}
