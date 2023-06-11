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
    }
}
