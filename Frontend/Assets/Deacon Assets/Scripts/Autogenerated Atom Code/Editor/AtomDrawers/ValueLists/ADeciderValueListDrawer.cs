#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.LDJ50.Editor
{
    /// <summary>
    /// Value List property drawer of type `ADecider`. Inherits from `AtomDrawer&lt;ADeciderValueList&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(ADeciderValueList))]
    public class ADeciderValueListDrawer : AtomDrawer<ADeciderValueList> { }
}
#endif
