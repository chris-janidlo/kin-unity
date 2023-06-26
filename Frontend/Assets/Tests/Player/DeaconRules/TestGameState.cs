using System.Linq;
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

        private static IEnumerable<TestCaseData> AvailableActionsData()
        {
            yield return new TestCaseData(
                new GameState(
                    new Board(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ S _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    Player.Blue
                ),
                new List<Board>
                {
                    new(
                        @"
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ P _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ P _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                }
            ).SetName("Single Scientist");

            yield return new TestCaseData(
                new GameState(
                    new Board(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ _ S _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    Player.Blue
                ),
                new List<Board>
                {
                    new(
                        @"
                            e s c i p
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ E _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ P _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ P _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            e s c i p
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                }
            ).SetName("Single Scientist With Enemies");

            yield return new TestCaseData(
                new GameState(
                    new Board(
                        @"
                            _ _ _ _ _
                            _ _ E _ _
                            I _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    Player.Blue
                ),
                new List<Board>
                {
                    new(
                        @"
                            _ I E _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ I P _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ I C _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ P E _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ P P _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ P C _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ C _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ C _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ E _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ P _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ C _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ E _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ P _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ C _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P E _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P P _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P C _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ P _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ C _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ C _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ I
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ I
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ C _ _ I
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ P
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ P
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ C _ _ P
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ C _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ E _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ P _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ C _ _
                        "
                    ),
                }
            ).SetName("Engineer And Pilot");

            yield return new TestCaseData(
                new GameState(
                    new Board(
                        @"
                            _ _ _ _ _
                            _ _ E _ _
                            P _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    Player.Blue
                ),
                new List<Board>
                {
                    new(
                        @"
                            E I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ R _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ R _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ R _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ R _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ R _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ R _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ R _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ R _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ R _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ R _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ E _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ R _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ R _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            E _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            E _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            E _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            R _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            R _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            R _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ I _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ P _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ E _ _ _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I E _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I R _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ E _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ R _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            E _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            R _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ I _ _ _
                            _ _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P E _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P R _ _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ E _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ R _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            E _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            R _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ P _ _ _
                            _ _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ R _ I _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ R _ P _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ R _ _ _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ R _ _ _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ R _ _ _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ R _ _ _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E I _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E P _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ E _ _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ R I _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ R P _
                            _ _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ R _ _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ R _ _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ R _ _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ R _ _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ E _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ E _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ E _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            E _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            R _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ I _
                            _ _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            E _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            R _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ P _
                            _ _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ R _
                            I _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ R _
                            P _ _ _ _
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ R _
                            _ _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ R _
                            _ _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            E _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            I _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            P _ _ _ _
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ I
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            R _ _ _ P
                            _ _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ I
                            R _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            E _ _ _ _

                        "
                    ),
                    new(
                        @"
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ P
                            R _ _ _ _

                        "
                    ),
                }
            ).SetName("Engineer and Priest, With Overlap");

            yield return new TestCaseData(
                new GameState(
                    new Board(
                        @"
                            _ _ _ r p
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    Player.Red
                ),
                new List<Board>
                {
                    new(
                        @"
                            _ _ _ _ e
                            _ _ _ _ r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ p
                            _ _ _ _ r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ c
                            _ _ _ _ r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ e
                            _ _ _ _ e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ p
                            _ _ _ _ e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ c
                            _ _ _ _ e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e r _ _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ p r _ _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ c r _ _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e e _ _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ p e _ _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ c e _ _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ r _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ p _ r _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ c _ r _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ e _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ p _ e _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ c _ e _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e r _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ p r _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ c r _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e e _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ p e _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ c e _
                            _ _ _ _ S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ r _ _ _
                            _ _ _ e S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ r _ _ _
                            _ _ _ p S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ r _ _ _
                            _ _ _ c S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ _ _
                            _ _ _ e S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ _ _
                            _ _ _ p S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ _ _
                            _ _ _ c S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ r _ _
                            _ _ _ e S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ r _ _
                            _ _ _ p S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ r _ _
                            _ _ _ c S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e _ _
                            _ _ _ e S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e _ _
                            _ _ _ p S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e _ _
                            _ _ _ c S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ r _
                            _ _ _ e S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ r _
                            _ _ _ p S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ r _
                            _ _ _ c S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ e _
                            _ _ _ e S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ e _
                            _ _ _ p S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ e _
                            _ _ _ c S
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ r _ _ _
                            _ _ _ _ S
                            _ _ _ e _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ r _ _ _
                            _ _ _ _ S
                            _ _ _ p _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ r _ _ _
                            _ _ _ _ S
                            _ _ _ c _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ _ _
                            _ _ _ _ S
                            _ _ _ e _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ _ _
                            _ _ _ _ S
                            _ _ _ p _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ _ _
                            _ _ _ _ S
                            _ _ _ c _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ r _ _
                            _ _ _ _ S
                            _ _ _ e _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ r _ _
                            _ _ _ _ S
                            _ _ _ p _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ r _ _
                            _ _ _ _ S
                            _ _ _ c _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e _ _
                            _ _ _ _ S
                            _ _ _ e _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e _ _
                            _ _ _ _ S
                            _ _ _ p _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e _ _
                            _ _ _ _ S
                            _ _ _ c _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ r _
                            _ _ _ _ S
                            _ _ _ e _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ r _
                            _ _ _ _ S
                            _ _ _ p _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ r _
                            _ _ _ _ S
                            _ _ _ c _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ e _
                            _ _ _ _ S
                            _ _ _ e _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ e _
                            _ _ _ _ S
                            _ _ _ p _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ e _
                            _ _ _ _ S
                            _ _ _ c _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ _ S
                            _ _ _ _ r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ p _ _ S
                            _ _ _ _ r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ c _ _ S
                            _ _ _ _ r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ e _ _ S
                            _ _ _ _ e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ p _ _ S
                            _ _ _ _ e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ c _ _ S
                            _ _ _ _ e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e _ S
                            _ _ _ _ r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ p _ S
                            _ _ _ _ r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ c _ S
                            _ _ _ _ r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ e _ S
                            _ _ _ _ e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ p _ S
                            _ _ _ _ e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ c _ S
                            _ _ _ _ e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ _ r
                            _ _ _ e _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ _ r
                            _ _ _ p _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ _ r
                            _ _ _ c _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ _ e
                            _ _ _ e _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ _ e
                            _ _ _ p _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ _ e
                            _ _ _ c _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ e r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ p r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ c r
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ e e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ p e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                    new(
                        @"
                            _ _ _ _ S
                            _ _ _ c e
                            _ _ _ _ _
                            _ _ _ _ _
                            _ _ _ _ _
                        "
                    ),
                }
            ).SetName("Swap Into Capture Maneuver");
        }

        [TestCaseSource(nameof(AvailableActionsData))]
        public void GetAvailableActionsGeneratesCorrectActions(
            GameState state,
            List<Board> expected
        )
        {
            var actions = new List<GameAction>();
            state.GetAvailableActions(actions);

            var actual = actions.Select(a => state.Board.ApplyAction(a)).ToList();

            Assert.That(actual, Has.Count.EqualTo(expected.Count));

            foreach (var action in expected)
            {
                Assert.That(actual, Has.Member(action));
            }
        }
    }
}
