// spice - in a 3D hexagonal grid (face-centered cubic), pieces gain territory by moving
// in the longest straight line available to them

mod direction;
mod grid;

use glam::*;
use ndarray::s;

use self::{direction::Direction, grid::{Grid, GridSpace}};
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

    fn apply_move(&self, move_: Self::Move) -> Self {
        let mut result = Self {
            grid: self.grid.clone(),
            player: match self.player {
                SpicePlayer::Red => SpicePlayer::Blue,
                SpicePlayer::Blue => SpicePlayer::Red,
            },
        };

        make_line(&mut result.grid, move_.source, move_.direction);

        result
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

fn make_line(grid: &mut Grid, source: IVec3, direction: Direction) {
    let og_player: SpicePlayer;
    let og_line_count: u8;
    if let GridSpace::Endpoint {
        owner,
        connected_lines,
    } = grid.get(source).unwrap()
    {
        og_player = owner;
        og_line_count = connected_lines;
    } else {
        unreachable!();
    }

    grid[source] = GridSpace::Endpoint {
        owner: og_player,
        connected_lines: og_line_count + 1,
    };

    let mut i = source;
    loop {
        match grid.get(i) {
            None => break,
            Some(space) => match space {
                GridSpace::Empty => grid[i] = GridSpace::Line(og_player, direction.axis()),
                GridSpace::Line(player, _) if player != og_player => clear_line_sliced(grid, i),
                _ => break,
            }
        }
        i += direction;
    }

    grid[i - direction] = GridSpace::Endpoint {
        owner: og_player,
        connected_lines: 1,
    };
}

fn clear_line_while(grid: &mut Grid, pos: IVec3) {
    let (up, down) = match grid[pos] {
        GridSpace::Line(_, axis) => axis.directions(),
        _ => unreachable!(),
    };

    // returns whether or not processing should continue
    let mut process_grid_space = |idx| {
        if let Some(space) = grid.get(idx) {
            match space {
                GridSpace::Line(_, _) => {
                    grid[idx] = GridSpace::Empty;
                    true
                }
                GridSpace::Endpoint {
                    owner,
                    connected_lines,
                } => {
                    let connected_lines = connected_lines - 1;
                    grid[idx] = if connected_lines > 0 {
                        GridSpace::Endpoint {
                            owner,
                            connected_lines,
                        }
                    } else {
                        GridSpace::Blocked
                    };
                    false
                }
                _ => unreachable!(),
            }
        } else {
            false
        }
    };

    let mut i = pos;
    while process_grid_space(i) {
        i += IVec3::from(up);
    }

    i = pos + IVec3::from(down);
    while process_grid_space(i) {
        i += IVec3::from(down);
    }
}

fn clear_line_sliced(grid: &mut Grid, pos: IVec3) {
    let (up, down) = match grid[pos] {
        GridSpace::Line(_, axis) => axis.vectors(),
        _ => unreachable!(),
    };

    let mut clear_slice = |slice| {
        for space in grid.slice_mut(slice) {
            match space {
                GridSpace::Line(..) => *space = GridSpace::Empty,
                GridSpace::Endpoint {
                    owner,
                    connected_lines,
                } => {
                    let connected_lines = *connected_lines - 1;
                    *space = if connected_lines > 0 {
                        GridSpace::Endpoint {
                            owner: *owner,
                            connected_lines,
                        }
                    } else {
                        GridSpace::Blocked
                    };
                    break;
                }
                _ => unreachable!(),
            }
        }
    };

    let up_slice = s![pos.x..;up.x, pos.y..;up.y, pos.z..;up.z];
    let down_slice = s![pos.x+down.x..;down.x, pos.y+down.y..;down.y, pos.z+down.z..;down.z];

    clear_slice(up_slice);
    clear_slice(down_slice);
}

#[cfg(test)]
mod tests {
    use rand::{self, Rng};
    use rstest::*;

    use super::{*, grid::GRID_CONSTANT_I};
    use crate::rules::spice::direction::Direction;

    fn random_position() -> IVec3 {
        let mut rng = rand::thread_rng();

        ivec3(
            rng.gen_range(-GRID_CONSTANT_I..=GRID_CONSTANT_I),
            rng.gen_range(-GRID_CONSTANT_I..=GRID_CONSTANT_I),
            rng.gen_range(-GRID_CONSTANT_I..=GRID_CONSTANT_I),
        )
    }

    fn random_player() -> SpicePlayer {
        match rand::random() {
            true => SpicePlayer::Blue,
            false => SpicePlayer::Red,
        }
    }

    #[fixture]
    fn empty_grid() -> Grid {
        Grid::default()
    }

    #[fixture]
    fn empty_grid_random_line(mut empty_grid: Grid) -> Grid {
        let direction: Direction = rand::random();
        let mut origin = random_position();
        while empty_grid.get_legal(origin).is_none() || empty_grid.get_legal(origin + direction).is_none() {
            origin = random_position();
        }

        empty_grid[origin] = GridSpace::Endpoint { owner: random_player(), connected_lines: 0 };
        make_line(&mut empty_grid, origin, direction);

        empty_grid
    }

    #[rstest]
    fn make_line_works(empty_grid_random_line: Grid) {}
}
