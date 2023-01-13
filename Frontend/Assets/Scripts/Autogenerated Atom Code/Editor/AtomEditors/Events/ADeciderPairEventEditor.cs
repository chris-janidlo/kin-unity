#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using Decisions;

namespace UnityAtoms.LDJ50.Editor
{
    /// <summary>
    /// Event property drawer of type `ADeciderPair`. Inherits from `AtomEventEditor&lt;ADeciderPair, ADeciderPairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(ADeciderPairEvent))]
    public sealed class ADeciderPairEventEditor : AtomEventEditor<ADeciderPair, ADeciderPairEvent> { }
}
#endif
