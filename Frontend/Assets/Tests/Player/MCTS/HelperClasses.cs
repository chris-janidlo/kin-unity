using System;
using System.Collections.Generic;
using Code.Player.MCTS;
using UnityEngine;

namespace Tests.Player.MCTS
{
    public static class TestMctsConstants
    {
        public static readonly double EXPLORATION_FACTOR = 1.0 / Math.Sqrt(2.0);
    }

    public enum TestPlayer
    {
        Black,
        White
    }

    public readonly struct TestAction : IGameAction
    {
        public readonly int Addend;

        public TestAction(int addend)
        {
            Addend = addend;
        }
    }

    public readonly struct TestState : IGameState<TestPlayer, TestState, TestAction>
    {
        public readonly int Value;

        public int ActionArrayMaxSize => 3;

        public TestState(int value)
        {
            Value = value;
        }

        public TestState ApplyAction(TestAction action)
        {
            return new TestState(Value + action.Addend);
        }

        public TestAction DefaultPolicy(IReadOnlyList<TestAction> actions)
        {
            return actions[UnityEngine.Random.Range(0, actions.Count)];
        }

        public void GetAvailableActions(IList<TestAction> buffer)
        {
            buffer.Add(new(1));
            buffer.Add(new(3));
            buffer.Add(new(5));
        }

        public TestPlayer NextToPlay()
        {
            return Value % 2 == 0 ? TestPlayer.Black : TestPlayer.White;
        }

        public double? ValueForPlayer(TestPlayer player)
        {
            if (Value < 10)
                return null;

            return player == NextToPlay() ? Value : -Value;
        }
    }
}
