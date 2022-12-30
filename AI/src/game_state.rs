use rand::{seq::IteratorRandom, thread_rng};

pub trait GameState: PartialEq + Sized {
    type Move;
    type Player: Copy;
    type MoveIterator: Iterator<Item = Self::Move>;

    fn initial_state() -> Self;

    fn available_moves(&self) -> Self::MoveIterator;

    fn next_to_play(&self) -> Self::Player;

    fn apply_move(&self, move_: Self::Move) -> Self;
    fn move_with_result(&self, result: &Self) -> Self::Move;

    fn default_policy(&self, moves: &mut impl Iterator<Item = Self::Move>) -> Option<Self> {
        moves
            .choose(&mut thread_rng())
            .map(|move_| self.apply_move(move_))
    }

    fn terminal_value(&self, for_player: Self::Player) -> Option<f32>;
}
