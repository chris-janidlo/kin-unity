using UnityEngine;
using UnityAtoms.BaseAtoms;
using Core_Rules;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Set variable value Action of type `GameState`. Inherits from `SetVariableValue&lt;GameState, GameStatePair, GameStateVariable, GameStateConstant, GameStateReference, GameStateEvent, GameStatePairEvent, GameStateVariableInstancer&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-purple")]
    [CreateAssetMenu(menuName = "Unity Atoms/Actions/Set Variable Value/GameState", fileName = "SetGameStateVariableValue")]
    public sealed class SetGameStateVariableValue : SetVariableValue<
        GameState,
        GameStatePair,
        GameStateVariable,
        GameStateConstant,
        GameStateReference,
        GameStateEvent,
        GameStatePairEvent,
        GameStateGameStateFunction,
        GameStateVariableInstancer>
    { }
}
