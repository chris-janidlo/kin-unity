using System;
using UnityEngine;
namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// IPair of type `&lt;ADecider&gt;`. Inherits from `IPair&lt;ADecider&gt;`.
    /// </summary>
    [Serializable]
    public struct ADeciderPair : IPair<ADecider>
    {
        public ADecider Item1 { get => _item1; set => _item1 = value; }
        public ADecider Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private ADecider _item1;
        [SerializeField]
        private ADecider _item2;

        public void Deconstruct(out ADecider item1, out ADecider item2) { item1 = Item1; item2 = Item2; }
    }
}
