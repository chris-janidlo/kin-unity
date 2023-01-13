#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.LDJ50.Editor
{
    /// <summary>
    /// Event property drawer of type `ADecider`. Inherits from `AtomDrawer&lt;ADeciderEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(ADeciderEvent))]
    public class ADeciderEventDrawer : AtomDrawer<ADeciderEvent> { }
}
#endif
