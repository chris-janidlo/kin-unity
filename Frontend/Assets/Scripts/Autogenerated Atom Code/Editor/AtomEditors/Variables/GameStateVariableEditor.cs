using UnityEditor;
using UnityAtoms.Editor;
using Core_Rules;

namespace UnityAtoms.LDJ50.Editor
{
    /// <summary>
    /// Variable Inspector of type `GameState`. Inherits from `AtomVariableEditor`
    /// </summary>
    [CustomEditor(typeof(GameStateVariable))]
    public sealed class GameStateVariableEditor : AtomVariableEditor<GameState, GameStatePair> { }
}
