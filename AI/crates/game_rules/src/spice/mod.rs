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
}

impl GameState for SpiceState {
    type Move = SpiceMove;

    type Player = SpicePlayer;

    type MoveIterator = SpiceMoveIterator;

    fn initial_state() -> Self {
        Self {
            grid: Default::default(),
            player: SpicePlayer::Blue,
        }
    }

    fn available_moves(&self) -> Self::MoveIterator {
        todo!()
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
        todo!()
    }
}
