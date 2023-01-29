use std::{f32::consts::FRAC_1_SQRT_2, time::Instant};

use humantime::format_duration;
use mcts::*;

#[derive(PartialEq)]
pub struct MockGameState(i32);

impl GameState for MockGameState {
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

pub fn main() {
    // warmup
    for _ in 0..10 {
        let mut searcher = Searcher::new(SearchParameters {
            exploration_factor: FRAC_1_SQRT_2,
            search_iterations: 10,
        });
        let _ = searcher.search(MockGameState(0));
    }

    // test
    let iterations = 1000;

    let now = Instant::now();

    for _ in 0..iterations {
        let mut searcher = Searcher::new(SearchParameters {
            exploration_factor: FRAC_1_SQRT_2,
            search_iterations: 10_000,
        });
        let _ = searcher.search(MockGameState(0));
    }

    let total = now.elapsed();
    let average = total / iterations;
    println!(
        "total time: {}, average time: {}",
        format_duration(total),
        format_duration(average)
    );
}
