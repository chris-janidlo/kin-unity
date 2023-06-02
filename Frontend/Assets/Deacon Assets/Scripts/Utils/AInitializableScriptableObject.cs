using UnityEngine;

namespace Utils
{
    public abstract class AInitializableScriptableObject : ScriptableObject
    {
        public abstract void Initialize();
    }
}
