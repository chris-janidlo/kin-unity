using System;
using UnityEngine.Events;
using Decisions;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// None generic Unity Event of type `ADecider`. Inherits from `UnityEvent&lt;ADecider&gt;`.
    /// </summary>
    [Serializable]
    public sealed class ADeciderUnityEvent : UnityEvent<ADecider> { }
}
