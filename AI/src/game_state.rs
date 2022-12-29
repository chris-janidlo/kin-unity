pub trait GameState: PartialEq {
    type Move;

    type MoveIterator: Iterator<Item = Self::Move>;

    fn initial_state() -> Self;

    // would prefer to use a return position `impl Trait` instead, but that isn't stable
    fn available_moves(&self) -> Self::MoveIterator;

    fn apply_move(&self, move_: Self::Move) -> Self;
}
