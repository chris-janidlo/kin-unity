using UnityEngine;
using Decisions;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Event Instancer of type `ADecider`. Inherits from `AtomEventInstancer&lt;ADecider, ADeciderEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/ADecider Event Instancer")]
    public class ADeciderEventInstancer : AtomEventInstancer<ADecider, ADeciderEvent> { }
}
