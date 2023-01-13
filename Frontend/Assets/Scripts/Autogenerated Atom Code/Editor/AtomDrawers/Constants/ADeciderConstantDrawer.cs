#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.LDJ50.Editor
{
    /// <summary>
    /// Constant property drawer of type `ADecider`. Inherits from `AtomDrawer&lt;ADeciderConstant&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(ADeciderConstant))]
    public class ADeciderConstantDrawer : VariableDrawer<ADeciderConstant> { }
}
#endif
