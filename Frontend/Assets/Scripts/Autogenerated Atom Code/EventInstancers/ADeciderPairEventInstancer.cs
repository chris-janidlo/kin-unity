using UnityEngine;
using Decisions;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event Instancer of type `ADeciderPair`. Inherits from `AtomEventInstancer&lt;ADeciderPair, ADeciderPairEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/ADeciderPair Event Instancer")]
    public class ADeciderPairEventInstancer
        : AtomEventInstancer<ADeciderPair, ADeciderPairEvent> { }
}
