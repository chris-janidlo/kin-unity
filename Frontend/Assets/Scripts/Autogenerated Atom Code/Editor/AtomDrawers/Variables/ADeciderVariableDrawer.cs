#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.LDJ50.Editor
{
    /// <summary>
    /// Variable property drawer of type `ADecider`. Inherits from `AtomDrawer&lt;ADeciderVariable&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(ADeciderVariable))]
    public class ADeciderVariableDrawer : VariableDrawer<ADeciderVariable> { }
}
#endif
