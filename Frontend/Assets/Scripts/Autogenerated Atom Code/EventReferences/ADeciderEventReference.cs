using System;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event Reference of type `ADecider`. Inherits from `AtomEventReference&lt;ADecider, ADeciderVariable, ADeciderEvent, ADeciderVariableInstancer, ADeciderEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class ADeciderEventReference : AtomEventReference<
        ADecider,
        ADeciderVariable,
        ADeciderEvent,
        ADeciderVariableInstancer,
        ADeciderEventInstancer>, IGetEvent
    { }
}
