using System.Collections.Generic;
using Code.Player.MCTS;
using UnityEngine;

namespace Code.Player.DeaconRules
{
    public readonly struct GameAction : IGameAction
    {
        public readonly List<ActionComponent> Components;

        public GameAction(List<ActionComponent> components)
        {
            Components = components;
        }

        public GameAction(params ActionComponent[] components)
        {
            Components = new List<ActionComponent>(components);
        }
    }

    public readonly struct ActionComponent
    {
        public readonly Vector2Int Origin;
        public readonly Vector2Int Destination;
        public readonly Form Transformation;

        public ActionComponent(Vector2Int origin, Vector2Int destination, Form transformation)
        {
            Origin = origin;
            Destination = destination;
            Transformation = transformation;
        }
    }
}
