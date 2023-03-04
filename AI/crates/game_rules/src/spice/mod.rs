// spice - in a 3D hexagonal grid (face-centered cubic), pieces gain territory by moving
// in the longest straight line available to them

mod coord;
mod direction;
mod grid;
mod moves;
mod players;

use mcts::GameState;

use self::{coord::*, direction::*, grid::*, moves::*, players::*};

const MAX_MOVES: u16 = 400;

#[derive(Debug, PartialEq)]
pub struct SpiceState {
    grid: Grid,
    player: SpicePlayer,
    move_cache: MoveCache,
    move_count: u16,
}

impl GameState for SpiceState {
    type Move = SpiceMove;

    type Player = SpicePlayer;

    type MoveIterator = std::vec::IntoIter<SpiceMove>;

    fn initial_state() -> Self {
        // definitions

        let player = SpicePlayer::Blue;

        let blue_endpoint_coords = vec![virt_d3(3, 3, 3)];
        let red_endpoint_coords = vec![virt_d3(-3, -3, -3)];

        // generation

        let mut grid: Grid = Default::default();

        for &index in &blue_endpoint_coords {
            grid.set_vc(
                index,
                GridSpace::Endpoint {
                    owner: SpicePlayer::Blue,
                    connected_lines: 0,
                },
            );
        }

        for &index in &red_endpoint_coords {
            grid.set_vc(
                index,
                GridSpace::Endpoint {
                    owner: SpicePlayer::Red,
                    connected_lines: 0,
                },
            );
        }

        let move_cache = MoveCache::from_grid(&grid);

        Self {
            grid,
            player,
            move_cache,
            move_count: 0,
        }
    }

    fn available_moves(&self) -> Self::MoveIterator {
        generate_moves(&self.grid, self.player, &self.move_cache).into_iter()
    }

    fn next_to_play(&self) -> Self::Player {
        self.player
    }

    fn apply_move(&self, move_: &Self::Move) -> Self {
        let mut grid = self.grid.clone();
        let mut move_cache = self.move_cache.clone();
        apply_move(&mut grid, &mut move_cache, move_, self.next_to_play());

        let player = match self.player {
            SpicePlayer::Red => SpicePlayer::Blue,
            SpicePlayer::Blue => SpicePlayer::Red,
        };

        Self {
            grid,
            player,
            move_cache,
            move_count: self.move_count + 1,
        }
    }

    fn terminal_value(&self, for_player: Self::Player) -> Option<f32> {
        #[inline]
        fn is_draw(state: &SpiceState) -> bool {
            state.move_count >= MAX_MOVES
                || out_of_moves(&state.grid, state.player, &state.move_cache)
        }

        if let Some(owner) = self.grid.center_owner() {
            Some(if owner == for_player { 1.0 } else { -1.0 })
        } else if is_draw(self) {
            Some(0.0)
        } else {
            None
        }
    }
}

#[cfg(test)]
mod tests {
    use mcts::*;
    use rstest::*;

    use super::*;

    #[rstest]
    fn search_runs_without_panic() {
        let mut searcher = Searcher::new(SearchParameters {
            exploration_factor: std::f32::consts::FRAC_1_SQRT_2,
            search_iterations: 10,
        });

        searcher.search(SpiceState::initial_state());
    }
}
