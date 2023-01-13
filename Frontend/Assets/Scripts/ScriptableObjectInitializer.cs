using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScriptableObjectInitializer : MonoBehaviour
{
    [FormerlySerializedAs("InitializableSOs")]
    public List<AInitializableScriptableObject> initializableScriptableObjects;

    private void Start()
    {
        foreach (var so in initializableScriptableObjects) so.Initialize();
    }
}
