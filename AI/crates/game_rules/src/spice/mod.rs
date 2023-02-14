// spice - in a 3D hexagonal grid (face-centered cubic), pieces gain territory by moving
// in the longest straight line available to them

mod coord;
mod direction;
mod grid;
mod moves;
mod players;

use mcts::GameState;

use self::{grid::*, moves::*, players::*};

#[derive(PartialEq)]
pub struct SpiceState {
    grid: Grid,
    player: SpicePlayer,
    moves: Vec<SpiceMove>,
}

impl GameState for SpiceState {
    type Move = SpiceMove;

    type Player = SpicePlayer;

    type MoveIterator = std::vec::IntoIter<SpiceMove>;

    fn initial_state() -> Self {
        let grid = Default::default();
        let player = SpicePlayer::Blue;
        let moves = generate_moves(&grid, player);

        Self {
            grid,
            player,
            moves,
        }
    }

    fn available_moves(&self) -> Self::MoveIterator {
        self.moves.clone().into_iter()
    }

    fn next_to_play(&self) -> Self::Player {
        self.player
    }

    fn apply_move(&self, move_: Self::Move) -> Self {
        todo!()
    }

    fn move_with_result(&self, result: &Self) -> Self::Move {
        todo!()
    }

    fn terminal_value(&self, for_player: Self::Player) -> Option<f32> {
        match self.moves.len() {
            0 if for_player == self.player => Some(-1.0),
            0 if for_player != self.player => Some(1.0),
            _ => None,
        }
    }
}
