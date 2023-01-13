using System;
using UnityAtoms.BaseAtoms;

namespace UnityAtoms.LDJ50
{
    /// <summary>
    /// Reference of type `ADecider`. Inherits from `EquatableAtomReference&lt;ADecider, ADeciderPair, ADeciderConstant, ADeciderVariable, ADeciderEvent, ADeciderPairEvent, ADeciderADeciderFunction, ADeciderVariableInstancer, AtomCollection, AtomList&gt;`.
    /// </summary>
    [Serializable]
    public sealed class ADeciderReference : EquatableAtomReference<
        ADecider,
        ADeciderPair,
        ADeciderConstant,
        ADeciderVariable,
        ADeciderEvent,
        ADeciderPairEvent,
        ADeciderADeciderFunction,
        ADeciderVariableInstancer>, IEquatable<ADeciderReference>
    {
        public ADeciderReference() : base() { }
        public ADeciderReference(ADecider value) : base(value) { }
        public bool Equals(ADeciderReference other) { return base.Equals(other); }
    }
}
