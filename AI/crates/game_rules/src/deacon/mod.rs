mod board;
mod form;
mod moves;

use board::*;
use form::*;
use mcts::GameState;
use moves::*;

#[derive(Debug, PartialEq, Eq, Clone, Copy, Hash, PartialOrd, Ord)]
pub enum Player {
    Red,
    Blue,
}

#[derive(Debug, PartialEq, Eq, Clone, Hash, PartialOrd, Ord)]
pub struct Piece {
    form: Form,
    owner: Player,
}

#[derive(PartialEq, Eq)]
pub struct State {
    to_play: Player,
    board: Board,
}

impl GameState for State {
    type Move = Move;

    type Player = Player;

    type MoveIterator = MoveIterator;

    fn initial_state() -> Self {
        Self {
            to_play: Player::Blue,
            board: Board::default(),
        }
    }

    fn available_moves(&self) -> Self::MoveIterator {
        MoveIterator::new(self)
    }

    fn next_to_play(&self) -> Self::Player {
        self.to_play
    }

    fn apply_move(&self, move_: &Self::Move) -> Self {
        let to_play = match self.to_play {
            Player::Red => Player::Blue,
            Player::Blue => Player::Red,
        };

        let mut board = self.board.clone();
        apply_move(&mut board, &move_);

        Self { to_play, board }
    }

    fn terminal_value(&self, for_player: Self::Player) -> Option<f32> {
        // assumes 2-player zero-sum
        let mut self_alive = false;
        let mut other_alive = false;

        for piece in self.board.pieces() {
            if piece.owner == for_player {
                self_alive = true;
            } else {
                other_alive = true;
            }

            if self_alive && other_alive {
                return None;
            }
        }

        if self_alive {
            Some(1.)
        } else {
            Some(-1.)
        }
    }
}
