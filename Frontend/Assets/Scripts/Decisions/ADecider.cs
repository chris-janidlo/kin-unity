using System;
using System.Threading;
using Core_Rules;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Decisions
{
    public abstract class ADecider : ScriptableObject, IEquatable<ADecider>
    {
        public virtual bool Deciding { get; protected set; }

        public bool Equals(ADecider other)
        {
            return base.Equals(other);
        }

        public abstract UniTask<GameState> DecideMove(GameState currentState, CancellationToken token);
    }
}
