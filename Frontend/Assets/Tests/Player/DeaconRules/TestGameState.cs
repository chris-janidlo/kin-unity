using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Code.Player.DeaconRules;

namespace Tests.Player.DeaconRules
{
    using Player = Code.Player.DeaconRules.Player;

    public class TestGameState
    {

        [TestCase(
            @"
                _ S _ _ _
                S _ _ _ _
                _ _ _ _ _
                _ _ _ _ s
                _ _ _ s _
            ",
            Player.Blue,
            null,
            TestName = "Initial For Blue"
        )]
        [TestCase(
            @"
                _ S _ _ _
                S _ _ _ _
                _ _ _ _ _
                _ _ _ _ s
                _ _ _ s _
            ",
            Player.Red,
            null,
            TestName = "Initial For Red"
        )]
        [TestCase(
            @"
                _ _ _ _ _
                _ _ P _ _
                _ _ i _ _
                E _ _ _ _
                _ r _ _ _
            ",
            Player.Blue,
            null,
            TestName = "Midgame For Blue"
        )]
        [TestCase(
            @"
                _ _ _ _ _
                _ _ P _ _
                _ _ i _ _
                E _ _ _ _
                _ r _ _ _
            ",
            Player.Red,
            null,
            TestName = "Midgame For Blue"
        )]
        [TestCase(
            @"
                _ _ _ _ _
                _ _ P _ _
                _ _ _ _ _
                E _ _ _ _
                _ _ _ _ _
            ",
            Player.Blue,
            1,
            TestName = "Blue Win, Blue Perspective"
        )]
        [TestCase(
            @"
                _ _ _ _ _
                _ _ P _ _
                _ _ _ _ _
                E _ _ _ _
                _ _ _ _ _
            ",
            Player.Red,
            -1,
            TestName = "Blue Win, Red Perspective"
        )]
        [TestCase(
            @"
                _ _ _ _ _
                _ _ p _ _
                _ _ _ _ _
                e _ _ _ _
                _ _ _ _ _
            ",
            Player.Blue,
            -1,
            TestName = "Red Win, Blue Perspective"
        )]
        [TestCase(
            @"
                _ _ _ _ _
                _ _ p _ _
                _ _ _ _ _
                e _ _ _ _
                _ _ _ _ _
            ",
            Player.Red,
            1,
            TestName = "Red Win, Red Perspective"
        )]
        public void ValueForPlayerIsAccurate(string layout, Player player, double? expected)
        {
            var state = new GameState(new Board(layout), player);
            var actual = state.ValueForPlayer(player);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
