using Code.Player.MCTS;
using Unity.Mathematics;
using UnityEngine;

namespace Code.Player.DeaconRules
{
    public readonly struct GameAction : IGameAction
    {
        public readonly ActionComponent First;
        public readonly ActionComponent Second;

        public GameAction(ActionComponent first, ActionComponent second)
        {
            First = first;
            Second = second;
        }
    }

    public readonly struct ActionComponent
    {
        public readonly int2 Origin;
        public readonly int2 Destination;
        public readonly Form Transformation;

        public ActionComponent(int2 origin, int2 destination, Form transformation)
        {
            Origin = origin;
            Destination = destination;
            Transformation = transformation;
        }
    }
}
