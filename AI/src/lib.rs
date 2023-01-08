use std::f32::consts::FRAC_1_SQRT_2;

mod game_state;
mod mcts;
mod rules;

use game_state::GameState;
use mcts::{SearchParameters, Searcher};

#[derive(PartialEq)]
pub struct GS(i32);

impl GameState for GS {
    type Move = i32;
    type Player = bool;
    type MoveIterator = std::vec::IntoIter<Self::Move>;

    fn initial_state() -> Self {
        Self(0)
    }

    fn available_moves(&self) -> Self::MoveIterator {
        vec![1, 3].into_iter()
    }

    fn next_to_play(&self) -> Self::Player {
        // `true` plays on even numbers, `false` plays on odd
        self.0 % 2 == 0
    }

    fn apply_move(&self, move_: Self::Move) -> Self {
        Self(self.0 + move_)
    }

    fn move_with_result(&self, result: &Self) -> Self::Move {
        result.0 - self.0
    }

    fn terminal_value(&self, for_player: Self::Player) -> Option<f32> {
        let score = if for_player == self.next_to_play() {
            self.0 as f32
        } else {
            -self.0 as f32
        };

        if score.abs() >= 10. {
            Some(score)
        } else {
            None
        }
    }
}

#[no_mangle]
pub extern "C" fn search() -> <GS as GameState>::Move {
    let mut searcher: Searcher<GS> = Searcher::new(SearchParameters {
        exploration_factor: FRAC_1_SQRT_2,
        search_iterations: 1000,
    });

    searcher.search(GS::initial_state())
}
