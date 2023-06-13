using Code.Player.DeaconRules;
using NUnit.Framework;

namespace Tests.Player.DeaconRules
{
    using Player = Code.Player.DeaconRules.Player;

    public class TestPiece
    {
        [Test]
        public void PackUnpackAreEquivalent(
            [Values(
                Form.Captain,
                Form.Engineer,
                Form.Pilot,
                Form.Priest,
                Form.Robot,
                Form.Scientist
            )]
                Form form,
            [Values(Player.Red, Player.Blue)] Player owner
        )
        {
            var expected = new Piece(form, owner);
            var actual = Piece.Unpack(Piece.Pack(expected));

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void PackUnpackNull()
        {
            Assert.That(Piece.Unpack(Piece.Pack(null)), Is.Null);
        }
    }
}
