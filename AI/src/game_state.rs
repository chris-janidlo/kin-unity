use rand::{seq::IteratorRandom, thread_rng};

pub trait GameState: PartialEq + Sized {
    type Move;
    type Player: Copy;
    type MoveIterator: Iterator<Item = Self::Move>;

    /// The state at the start of a game.
    fn initial_state() -> Self;

    /// The moves that are legal for the current player to take.
    fn available_moves(&self) -> Self::MoveIterator;

    /// In more words, the player whose turn it is.
    fn next_to_play(&self) -> Self::Player;

    /// Expects move to be legal for this state. Might do weird things, including
    /// returning illegal game states and panicking, if passed an illegal move - the
    /// implementation is not expected to check for that.
    fn apply_move(&self, move_: Self::Move) -> Self;

    /// Expects the result state to be legal and reachable from this state. Like
    /// [GameState::apply_move], might do weird things otherwise.
    fn move_with_result(&self, result: &Self) -> Self::Move;

    /// Used in simulations and expansion to choose either promising or pseudo-random
    /// moves.
    fn default_policy(&self, moves: &mut impl Iterator<Item = Self::Move>) -> Option<Self> {
        moves
            .choose(&mut thread_rng())
            .map(|move_| self.apply_move(move_))
    }

    /// Returns [None] if this state is non-terminal (ie, the game is still going on),
    /// and [Some(value)] if the game has finished and this state is worth `value` to the
    /// player whose turn it is.
    fn terminal_value(&self, for_player: Self::Player) -> Option<f32>;
}
