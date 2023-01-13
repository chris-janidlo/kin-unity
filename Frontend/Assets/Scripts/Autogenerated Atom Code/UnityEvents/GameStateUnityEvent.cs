using System;
using UnityEngine.Events;
using Core_Rules;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// None generic Unity Event of type `GameState`. Inherits from `UnityEvent&lt;GameState&gt;`.
    /// </summary>
    [Serializable]
    public sealed class GameStateUnityEvent : UnityEvent<GameState> { }
}
