using UnityEngine;
using UnityAtoms.BaseAtoms;
using Core_Rules;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Adds Variable Instancer's Variable of type GameState to a Collection or List on OnEnable and removes it on OnDestroy.
    /// </summary>
    [AddComponentMenu(
        "Unity Atoms/Sync Variable Instancer to Collection/Sync GameState Variable Instancer to Collection"
    )]
    [EditorIcon("atom-icon-delicate")]
    public class SyncGameStateVariableInstancerToCollection
        : SyncVariableInstancerToCollection<
            GameState,
            GameStateVariable,
            GameStateVariableInstancer
        > { }
}
