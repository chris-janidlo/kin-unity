use std::{f32::consts::FRAC_1_SQRT_2, time::Instant};

use game_rules::deacon::State;
use humantime::format_duration;
use mcts::*;

pub fn main() {
    // warmup
    for _ in 0..10 {
        let mut searcher = Searcher::new(SearchParameters {
            exploration_factor: FRAC_1_SQRT_2,
            search_iterations: 10,
        });
        let _ = searcher.search(State::initial_state());
    }

    // test
    let iterations = 1000;

    let now = Instant::now();

    for _ in 0..iterations {
        let mut searcher = Searcher::new(SearchParameters {
            exploration_factor: FRAC_1_SQRT_2,
            search_iterations: 10_000,
        });
        let _ = searcher.search(State::initial_state());
    }

    let total = now.elapsed();
    let average = total / iterations;
    println!(
        "total time: {}, average time: {}",
        format_duration(total),
        format_duration(average)
    );
}
