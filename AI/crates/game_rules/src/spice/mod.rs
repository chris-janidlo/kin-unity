// spice - in a 3D hexagonal grid (face-centered cubic), pieces gain territory by moving
// in the longest straight line available to them

mod coord;
mod direction;
mod grid;
mod moves;
mod players;

use mcts::GameState;

use self::{coord::*, direction::*, grid::*, moves::*, players::*};

#[derive(Debug, PartialEq)]
pub struct SpiceState {
    grid: Grid,
    player: SpicePlayer,
    moves: Vec<SpiceMove>,
    move_cache: MoveCache,
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
        let moves = generate_moves(&grid, player, &move_cache);

        Self {
            grid,
            player,
            moves,
            move_cache,
        }
    }

    fn available_moves(&self) -> Self::MoveIterator {
        self.moves.clone().into_iter()
    }

    fn next_to_play(&self) -> Self::Player {
        self.player
    }

    fn apply_move(&self, move_: Self::Move) -> Self {
        let mut grid = self.grid.clone();
        let mut move_cache = self.move_cache.clone();
        apply_move(&mut grid, &mut move_cache, move_, self.next_to_play());

        let player = match self.player {
            SpicePlayer::Red => SpicePlayer::Blue,
            SpicePlayer::Blue => SpicePlayer::Red,
        };

        let moves = generate_moves(&grid, player, &move_cache);

        Self {
            grid,
            player,
            moves,
            move_cache,
        }
    }

    fn move_with_result(&self, result: &Self) -> Self::Move {
        // TODO: this is a dummy implementation

        println!("{result:?}");

        SpiceMove {
            source: virt_d3(0, 0, 0),
            direction: Direction::DownEast,
        }
    }

    fn terminal_value(&self, for_player: Self::Player) -> Option<f32> {
        match self.moves.len() {
            0 if for_player == self.player => Some(-1.0),
            0 if for_player != self.player => Some(1.0),
            _ => None,
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
