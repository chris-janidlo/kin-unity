using UnityEditor;
using UnityAtoms.Editor;
using Decisions;

namespace UnityAtoms.LDJ50.Editor
{
    /// <summary>
    /// Variable Inspector of type `ADecider`. Inherits from `AtomVariableEditor`
    /// </summary>
    [CustomEditor(typeof(ADeciderVariable))]
    public sealed class ADeciderVariableEditor : AtomVariableEditor<ADecider, ADeciderPair> { }
}