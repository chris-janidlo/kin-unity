using System;

namespace Code.Player.MCTS
{
    [Serializable]
    public struct SearchParameters
    {
        public double ExplorationFactor;
        public int Iterations;
    }
}
