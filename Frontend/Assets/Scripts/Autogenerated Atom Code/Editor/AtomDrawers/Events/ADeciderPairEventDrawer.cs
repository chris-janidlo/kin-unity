#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.LDJ50.Editor
{
    /// <summary>
    /// Event property drawer of type `ADeciderPair`. Inherits from `AtomDrawer&lt;ADeciderPairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(ADeciderPairEvent))]
    public class ADeciderPairEventDrawer : AtomDrawer<ADeciderPairEvent> { }
}
#endif
