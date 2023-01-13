#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;

namespace UnityAtoms.LDJ50.Editor
{
    /// <summary>
    /// Event property drawer of type `ADecider`. Inherits from `AtomEventEditor&lt;ADecider, ADeciderEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(ADeciderEvent))]
    public sealed class ADeciderEventEditor : AtomEventEditor<ADecider, ADeciderEvent> { }
}
#endif
