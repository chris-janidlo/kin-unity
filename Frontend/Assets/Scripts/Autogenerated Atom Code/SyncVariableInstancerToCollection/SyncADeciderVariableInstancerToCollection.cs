using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Adds Variable Instancer's Variable of type ADecider to a Collection or List on OnEnable and removes it on OnDestroy.
    /// </summary>
    [AddComponentMenu("Unity Atoms/Sync Variable Instancer to Collection/Sync ADecider Variable Instancer to Collection")]
    [EditorIcon("atom-icon-delicate")]
    public class SyncADeciderVariableInstancerToCollection : SyncVariableInstancerToCollection<ADecider, ADeciderVariable, ADeciderVariableInstancer> { }
}
