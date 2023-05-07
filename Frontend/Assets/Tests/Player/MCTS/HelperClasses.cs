using Code.Player.MCTS;
using Tests.Player.MCTS;
using Unity.Collections;
using Unity.Jobs;

[assembly: RegisterGenericJobType(typeof(RolloutJob<TestPlayer, TestState, TestAction>))]

namespace Tests.Player.MCTS
{
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

        public TestAction DefaultPolicy(NativeList<TestAction> actions)
        {
            return actions[actions.Length / 2];
        }

        public void GetAvailableActions(ref NativeList<TestAction> buffer)
        {
            buffer.AddNoResize(new TestAction(1));
            buffer.AddNoResize(new TestAction(3));
            buffer.AddNoResize(new TestAction(5));
        }

        public TestPlayer NextToPlay()
        {
            return Value % 2 == 0
                ? TestPlayer.Black
                : TestPlayer.White;
        }

        public double? ValueForPlayer(TestPlayer player)
        {
            if (Value < 10) return null;

            return player == NextToPlay()
                ? Value
                : -Value;
        }
    }
}