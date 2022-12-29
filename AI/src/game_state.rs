use rand::{seq::IteratorRandom, thread_rng};

pub trait GameState: PartialEq + Sized {
    type Move;

    type MoveIterator: Iterator<Item = Self::Move>;

    fn initial_state() -> Self;

    // would prefer to use a return position `impl Trait` instead, but that isn't stable
    fn available_moves(&self) -> Self::MoveIterator;

    fn apply_move(&self, move_: Self::Move) -> Self;

    fn default_policy(&self, moves: impl Iterator<Item = Self::Move>) -> Option<Self> {
        moves
            .choose(&mut thread_rng())
            .map(|move_| self.apply_move(move_))
    }

    fn terminal_value(&self) -> Option<f32>;
}
