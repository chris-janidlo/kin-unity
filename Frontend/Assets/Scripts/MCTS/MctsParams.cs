using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MCTS
{
    [Serializable]
    public class MctsParameters
    {
        [FormerlySerializedAs("Iterations")]
        [Min(2)]
        public int iterations;

        [FormerlySerializedAs("ExplorationFactor")]
        [Min(0)]
        public float explorationFactor;
    }
}
