using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Player.DeaconRules;
using NUnit.Framework;
using crass;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tests.Player.DeaconRules
{
    using Player = Code.Player.DeaconRules.Player;

    public class TestBoard
    {
        private static IEnumerable<TestCaseData> LayoutTestData()
        {
            yield return new TestCaseData(
                @"
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                ",
                Board.EMPTY
            ).SetName("Empty Board");

            yield return new TestCaseData(
                @"
                    _ S _ _ _
                    S _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ s
                    _ _ _ s _
                ",
                Board.INITIAL
            ).SetName("Initial Board");

            yield return new TestCaseData(
                @"
                    C c E e _
                    I i P p _
                    R r _ _ _
                    _ _ S s _
                    _ _ _ _ _
                ",
                new Board(
                    new[]
                    {
                        (0, 0, new Piece(Form.Captain, Player.Blue)),
                        (1, 0, new Piece(Form.Captain, Player.Red)),
                        (2, 0, new Piece(Form.Engineer, Player.Blue)),
                        (3, 0, new Piece(Form.Engineer, Player.Red)),
                        (0, 1, new Piece(Form.Pilot, Player.Blue)),
                        (1, 1, new Piece(Form.Pilot, Player.Red)),
                        (2, 1, new Piece(Form.Priest, Player.Blue)),
                        (3, 1, new Piece(Form.Priest, Player.Red)),
                        (0, 2, new Piece(Form.Robot, Player.Blue)),
                        (1, 2, new Piece(Form.Robot, Player.Red)),
                        (2, 3, new Piece(Form.Scientist, Player.Blue)),
                        (3, 3, new Piece(Form.Scientist, Player.Red)),
                    }
                )
            ).SetName("Full House");

            yield return new TestCaseData(
                @"
                _ _  _ _ _
                    _ _ _  _ _  
            _ _ _   _ _
        _    _ _ _ _   
                        _   _ _ _ _
                ",
                Board.EMPTY
            ).SetName("Jagged Whitespace");
        }

        [TestCaseSource(nameof(LayoutTestData))]
        public void LayoutWorks(string layout, Board expected)
        {
            var parsed = new Board(layout);
            Assert.That(parsed, Is.EqualTo(expected));

            var cleanedLayoutLines = layout
                .Split(Environment.NewLine)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(line =>
                {
                    var chars = line.Split().Where(c => !string.IsNullOrWhiteSpace(c));
                    return string.Join(" ", chars);
                });

            string cleanedLayout = string.Join(Environment.NewLine, cleanedLayoutLines);
            Assert.That(parsed.ToString(), Is.EqualTo(cleanedLayout));
        }

        [TestCase("", TestName = "Empty String")]
        [TestCase("o47afeujh8", TestName = "Garbage String")]
        [TestCase(
            @"
                _ _ _ _ _
                _ _ _ _ _
                _ _ _ _ _
                _ _ _ _ _
            ",
            TestName = "Too Few Rows"
        )]
        [TestCase(
            @"
                _ _ _ _
                _ _ _ _
                _ _ _ _
                _ _ _ _
                _ _ _ _
            ",
            TestName = "Too Few Columns"
        )]
        [TestCase(
            @"
                _ _ _ _ _
                _ _ _ _ _ _
                _ _ _ _ _
                _ _ _ _
                _ _ _ _ _
            ",
            TestName = "Jagged Array"
        )]
        [TestCase(
            @"
                _ _ _ _ _
                _ _ _ _ _
                _ _ _ _ _
                _ _ _ 3 _
                _ _ _ _ _
            ",
            TestName = "Unexpected Character"
        )]
        [TestCase(
            @"
                _ _ _ _ _
                _ Ie _ _
                _ _ _ _ _
                _ _ _ _ _
                _ _ _ _ _
            ",
            TestName = "Bad Spacing"
        )]
        public void ConstructorComplainsAboutBadLayout(string layout)
        {
            Assert.That(() => new Board(layout), Throws.ArgumentException);
        }

        private static IEnumerable<TestCaseData> ApplyActionComponentTestData()
        {
            yield return new TestCaseData(
                new Board(
                    @"
                    S _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                "
                ),
                new ActionComponent(Vector2Int.zero, Vector2Int.one, Form.Engineer),
                new Board(
                    @"
                    _ _ _ _ _
                    _ E _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                "
                )
            ).SetName("Basic Scientist Movement");

            yield return new TestCaseData(
                new Board(
                    @"
                    S _ r _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                "
                ),
                new ActionComponent(Vector2Int.zero, Vector2Int.one, Form.Engineer),
                new Board(
                    @"
                    _ _ r _ _
                    _ E _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                "
                )
            ).SetName("Basic Scientist Movement With Enemy On Board");

            yield return new TestCaseData(
                new Board(
                    @"
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ E _ _
                    _ _ _ _ _
                    s _ _ _ _
                "
                ),
                new ActionComponent(new(0, 4), new(2, 2), Form.Scientist),
                new Board(
                    @"
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ s _ _
                    _ _ _ _ _
                    _ _ _ _ _
                "
                )
            ).SetName("Capture");

            yield return new TestCaseData(
                new Board(
                    @"
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ c _ _
                    _ _ _ _ _
                    _ _ P _ _
                "
                ),
                new ActionComponent(new(2, 4), new(2, 2), Form.Engineer),
                new Board(
                    @"
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ E _ _
                    _ _ _ _ _
                    _ _ c _ _
                "
                )
            ).SetName("Swap");
        }

        [TestCaseSource(nameof(ApplyActionComponentTestData))]
        public void ApplyActionComponentWorks(
            Board start,
            ActionComponent actionComponent,
            Board expected
        )
        {
            var array = (byte[])start.pieces_packed.Clone();
            Board.ApplyActionComponent(array, actionComponent);
            var actual = new Board(array);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private static IEnumerable<TestCaseData> ApplyGameActionTestData()
        {
            yield return new TestCaseData(
                new Board(
                    @"
                    _ S _ _ _
                    S _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                "
                ),
                new GameAction(
                    new ActionComponent(new(0, 1), new(1, 1), Form.Engineer),
                    new ActionComponent(new(1, 0), new(0, 0), Form.Priest)
                ),
                new Board(
                    @"
                    P _ _ _ _
                    _ E _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                "
                )
            ).SetName("Basic Double Movement");

            yield return new TestCaseData(
                new Board(
                    @"
                    _ _ _ _ _
                    _ _ _ E _
                    _ _ _ _ _
                    _ I _ _ c
                    _ _ _ c _
                "
                ),
                new GameAction(
                    new ActionComponent(new(3, 1), new(2, 0), Form.Pilot),
                    new ActionComponent(new(1, 3), new(0, 2), Form.Captain)
                ),
                new Board(
                    @"
                    _ _ I _ _
                    _ _ _ _ _
                    C _ _ _ _
                    _ _ _ _ c
                    _ _ _ c _
                "
                )
            ).SetName("Double Movement With Enemies On Board");

            yield return new TestCaseData(
                new Board(
                    @"
                    _ _ _ _ _
                    _ _ _ r _
                    _ _ _ _ _
                    _ i _ S _
                    _ _ C _ _
                "
                ),
                new GameAction(
                    new ActionComponent(new(3, 1), new(3, 3), Form.Priest),
                    new ActionComponent(new(1, 3), new(2, 4), Form.Priest)
                ),
                new Board(
                    @"
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ _ _
                    _ _ _ p _
                    _ _ p _ _
                "
                )
            ).SetName("Double Capture");

            yield return new TestCaseData(
                new Board(
                    @"
                    _ _ _ _ _
                    _ e _ r _
                    _ _ _ _ _
                    _ _ _ S _
                    _ _ _ _ _
                "
                ),
                new GameAction(
                    new ActionComponent(new(1, 1), new(3, 3), Form.Pilot),
                    new ActionComponent(new(3, 1), new(1, 1), Form.Engineer)
                ),
                new Board(
                    @"
                    _ _ _ _ _
                    _ e _ _ _
                    _ _ _ _ _
                    _ _ _ i _
                    _ _ _ _ _
                "
                )
            ).SetName("Basic Double Movement");
        }

        [TestCaseSource(nameof(ApplyGameActionTestData))]
        public void ApplyGameActionWorks(Board start, GameAction action, Board expected)
        {
            var actual = start.ApplyAction(action);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
