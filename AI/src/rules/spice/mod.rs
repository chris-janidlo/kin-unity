// spice - in a 3D hexagonal grid (face-centered cubic), pieces gain territory by moving
// in the longest straight line available to them

mod direction;
mod grid;

use glam::*;

use self::{direction::Direction, grid::Grid};
use crate::game_state::GameState;

#[derive(PartialEq)]
pub struct SpiceState {
    grid: Grid,
    player: SpicePlayer,
}

pub struct SpiceMove {
    source: IVec3,
    direction: Direction,
}

#[derive(Debug, PartialEq, Eq, Clone, Copy)]
pub enum SpicePlayer {
    Red,
    Blue,
}

pub struct SpiceMoveIterator {}

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

    fn apply_move(&self, _move_: Self::Move) -> Self {
        todo!()
    }

    fn move_with_result(&self, _result: &Self) -> Self::Move {
        todo!()
    }

    fn terminal_value(&self, _for_player: Self::Player) -> Option<f32> {
        todo!()
    }
}

impl Iterator for SpiceMoveIterator {
    type Item = SpiceMove;

    fn next(&mut self) -> Option<Self::Item> {
        todo!()
    }
}
