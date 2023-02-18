use mcts::GameState;

use super::{coord::*, direction::*, grid::*, players::*, SpiceState};

#[derive(Debug, PartialEq, Eq, Clone)]
pub struct SpiceMove {
    pub(crate) source: VirtD3,
    pub(crate) direction: Direction,
}

pub fn generate_moves(grid: &Grid, player: SpicePlayer) -> Vec<SpiceMove> {
    let owned_endpoint_coords = grid.enumerate_vc().filter_map(|(c, s)| match s {
        GridSpace::Endpoint { owner, .. } if *owner == player => Some(c),
        _ => None,
    });

    owned_endpoint_coords
        .flat_map(|c| {
            Direction::ALL.iter().filter_map(move |&d| {
                grid.get_vc(c + d).and_then(|s| match s {
                    GridSpace::Empty => Some(SpiceMove {
                        source: c,
                        direction: d,
                    }),
                    _ => None,
                })
            })
        })
        .collect()
}

pub fn apply_move(grid: &mut Grid, move_: SpiceMove, player: SpicePlayer) {
    // TODO: use anyhow instead of immediately panicking?

    // this function (and the tree of subroutines it invokes) could probably be done with
    // some kind of slice over the contents of grid, but I was running into issues with
    // lifetimes when implementing a method in Grid for it. unsure if that would be more
    // performant, but this is probably fine as is

    update_start_endpoint(grid, move_.source);

    let axis = move_.direction.axis();
    let mut ray_coord = move_.source + move_.direction;
    while let Some(space) = grid.get_vc(ray_coord) {
        match space {
            GridSpace::Empty => {
                set_line_segment(grid, ray_coord, axis, false);
            }

            GridSpace::LineSegment { axis: ax, hardened } => {
                if *hardened {
                    break;
                }

                let (dir1, dir2) = ax.directions();

                cut_line_in_direction(grid, ray_coord + dir1, dir1);
                cut_line_in_direction(grid, ray_coord + dir2, dir2);

                set_line_segment(grid, ray_coord, axis, true);
            }

            GridSpace::Blocked | GridSpace::Endpoint { .. } => break,
        }

        ray_coord += move_.direction;
    }

    let end_coord = ray_coord - move_.direction;
    create_end_endpoint(grid, end_coord, player);
}

fn update_start_endpoint(grid: &mut Grid, coord: VirtD3) {
    let mut gs = grid
        .get_mut_vc(coord)
        .expect("apply_move source should be a valid grid point");

    if let GridSpace::Endpoint {
        connected_lines, ..
    } = gs
    {
        *connected_lines += 1;
    } else {
        panic!("can only apply_move if source is an endpoint");
    }
}

fn create_end_endpoint(grid: &mut Grid, coord: VirtD3, player: SpicePlayer) {
    let endpoint = GridSpace::Endpoint {
        owner: player,
        connected_lines: 1,
    };

    grid.set_vc(coord, endpoint)
        .expect("should be able to set the end endpoint");
}

fn set_line_segment(grid: &mut Grid, coord: VirtD3, axis: Axis, hardened: bool) {
    grid.set_vc(coord, GridSpace::LineSegment { axis, hardened })
        .expect("should be able to set a line segment");
}

fn cut_line_in_direction(grid: &mut Grid, start_coord: VirtD3, dir: Direction) {
    let mut coord = start_coord;
    while let Some(space) = grid.get_vc(coord) {
        match space {
            GridSpace::LineSegment { .. } => {
                grid.set_vc(coord, GridSpace::Empty)
                    .expect("should be able to delete a line segment");
            }

            GridSpace::Endpoint {
                owner,
                connected_lines,
            } => {
                let new_space = if *connected_lines == 1 {
                    GridSpace::Blocked
                } else {
                    GridSpace::Endpoint {
                        owner: *owner,
                        connected_lines: connected_lines - 1,
                    }
                };

                grid.set_vc(coord, new_space)
                    .expect("should be able to update an endpoint after clearing a line");

                break;
            }

            _ => panic!("should only see lines or endpoints when clearing a line"),
        }

        coord += dir;
    }
}

#[cfg(test)]
mod tests {
    use pretty_assertions::assert_eq;
    use rstest::*;

    use super::*;
    use crate::spice::players::*;

    #[fixture]
    fn empty_grid() -> Grid {
        Grid::default()
    }

    fn sort_move_list(moves: &mut [SpiceMove]) {
        moves.sort_unstable_by_key(|m| {
            let VirtD3 {
                i: si,
                j: sj,
                k: sk,
            } = m.source;

            let VirtD3 {
                i: di,
                j: dj,
                k: dk,
            } = m.direction.into();

            ((si, sj, sk), (di, dj, dk))
        });
    }

    fn smove(s: VirtD3, d: Direction) -> SpiceMove {
        SpiceMove {
            source: s,
            direction: d,
        }
    }

    #[rstest]
    #[case(
        SpicePlayer::Blue,
        vec![],
        vec![]
    )]
    #[case(
        SpicePlayer::Red,
        vec![
            (virt_d3(0, 0, 0), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 })
        ],
        vec![]
    )]
    #[case(
        SpicePlayer::Blue,
        vec![
            (virt_d3(0, 0, 0), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 })
        ],
        vec![
            smove(virt_d3(0, 0, 0), Direction::NorthEast),
            smove(virt_d3(0, 0, 0), Direction::NorthWest),
            smove(virt_d3(0, 0, 0), Direction::SouthEast),
            smove(virt_d3(0, 0, 0), Direction::SouthWest),
            smove(virt_d3(0, 0, 0), Direction::UpNorth),
            smove(virt_d3(0, 0, 0), Direction::UpSouth),
            smove(virt_d3(0, 0, 0), Direction::UpEast),
            smove(virt_d3(0, 0, 0), Direction::UpWest),
            smove(virt_d3(0, 0, 0), Direction::DownNorth),
            smove(virt_d3(0, 0, 0), Direction::DownSouth),
            smove(virt_d3(0, 0, 0), Direction::DownEast),
            smove(virt_d3(0, 0, 0), Direction::DownWest),
        ]
    )]
    #[case(
        SpicePlayer::Blue,
        vec![
            (virt_d3(3, 3, 3), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 })
        ],
        vec![
            smove(virt_d3(3, 3, 3), Direction::SouthWest),
            smove(virt_d3(3, 3, 3), Direction::DownSouth),
            smove(virt_d3(3, 3, 3), Direction::DownWest),
        ]
    )]
    #[case(
        SpicePlayer::Red,
        vec![
            (virt_d3(-3, -3, -3), GridSpace::Endpoint { owner: SpicePlayer::Red, connected_lines: 0 })
        ],
        vec![
            smove(virt_d3(-3, -3, -3), Direction::NorthEast),
            smove(virt_d3(-3, -3, -3), Direction::UpNorth),
            smove(virt_d3(-3, -3, -3), Direction::UpEast),
        ]
    )]
    #[case(
        SpicePlayer::Red,
        vec![
            (virt_d3(0, 0, 0), GridSpace::Endpoint { owner: SpicePlayer::Red, connected_lines: 0 }),
            (virt_d3(0, 0, 1), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(1, -1, 0), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(-1, 1, 0), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(0, 0, -1), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
        ],
        vec![
            smove(virt_d3(0, 0, 0), Direction::UpNorth),
            smove(virt_d3(0, 0, 0), Direction::UpSouth),
            smove(virt_d3(0, 0, 0), Direction::UpEast),
            smove(virt_d3(0, 0, 0), Direction::UpWest),
            smove(virt_d3(0, 0, 0), Direction::DownNorth),
            smove(virt_d3(0, 0, 0), Direction::DownSouth),
            smove(virt_d3(0, 0, 0), Direction::DownEast),
            smove(virt_d3(0, 0, 0), Direction::DownWest),
        ]
    )]
    #[case(
        SpicePlayer::Blue,
        vec![
            (virt_d3(1, 1, 1), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(-1, -1, -1), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
        ],
        vec![
            smove(virt_d3(1, 1, 1), Direction::NorthEast),
            smove(virt_d3(1, 1, 1), Direction::NorthWest),
            smove(virt_d3(1, 1, 1), Direction::SouthEast),
            smove(virt_d3(1, 1, 1), Direction::SouthWest),
            smove(virt_d3(1, 1, 1), Direction::UpNorth),
            smove(virt_d3(1, 1, 1), Direction::UpSouth),
            smove(virt_d3(1, 1, 1), Direction::UpEast),
            smove(virt_d3(1, 1, 1), Direction::UpWest),
            smove(virt_d3(1, 1, 1), Direction::DownNorth),
            smove(virt_d3(1, 1, 1), Direction::DownSouth),
            smove(virt_d3(1, 1, 1), Direction::DownEast),
            smove(virt_d3(1, 1, 1), Direction::DownWest),
            smove(virt_d3(-1, -1, -1), Direction::NorthEast),
            smove(virt_d3(-1, -1, -1), Direction::NorthWest),
            smove(virt_d3(-1, -1, -1), Direction::SouthEast),
            smove(virt_d3(-1, -1, -1), Direction::SouthWest),
            smove(virt_d3(-1, -1, -1), Direction::UpNorth),
            smove(virt_d3(-1, -1, -1), Direction::UpSouth),
            smove(virt_d3(-1, -1, -1), Direction::UpEast),
            smove(virt_d3(-1, -1, -1), Direction::UpWest),
            smove(virt_d3(-1, -1, -1), Direction::DownNorth),
            smove(virt_d3(-1, -1, -1), Direction::DownSouth),
            smove(virt_d3(-1, -1, -1), Direction::DownEast),
            smove(virt_d3(-1, -1, -1), Direction::DownWest),
        ]
    )]
    #[case(
        SpicePlayer::Red,
        vec![
            (virt_d3(0, 0, 0), GridSpace::Endpoint { owner: SpicePlayer::Red, connected_lines: 0 }),
            (virt_d3(0, 0, 1), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(1, -1, 0), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(-1, 1, 0), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(0, 0, -1), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            //
            (virt_d3(2, 2, 2), GridSpace::Endpoint { owner: SpicePlayer::Red, connected_lines: 0 }),
            (virt_d3(2, 2, 3), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(3, 1, 2), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(1, 3, 2), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
            (virt_d3(2, 2, 1), GridSpace::Endpoint { owner: SpicePlayer::Blue, connected_lines: 0 }),
        ],
        vec![
            smove(virt_d3(0, 0, 0), Direction::UpNorth),
            smove(virt_d3(0, 0, 0), Direction::UpSouth),
            smove(virt_d3(0, 0, 0), Direction::UpEast),
            smove(virt_d3(0, 0, 0), Direction::UpWest),
            smove(virt_d3(0, 0, 0), Direction::DownNorth),
            smove(virt_d3(0, 0, 0), Direction::DownSouth),
            smove(virt_d3(0, 0, 0), Direction::DownEast),
            smove(virt_d3(0, 0, 0), Direction::DownWest),
            //
            smove(virt_d3(2, 2, 2), Direction::UpNorth),
            smove(virt_d3(2, 2, 2), Direction::UpSouth),
            smove(virt_d3(2, 2, 2), Direction::UpEast),
            smove(virt_d3(2, 2, 2), Direction::UpWest),
            smove(virt_d3(2, 2, 2), Direction::DownNorth),
            smove(virt_d3(2, 2, 2), Direction::DownSouth),
            smove(virt_d3(2, 2, 2), Direction::DownEast),
            smove(virt_d3(2, 2, 2), Direction::DownWest),
        ]
    )]
    #[case(
        SpicePlayer::Red,
        vec![
            (virt_d3(0, 0, 0), GridSpace::Endpoint { owner: SpicePlayer::Red, connected_lines: 0 }),
            (virt_d3(1, 0, -1), GridSpace::Blocked),
            (virt_d3(0, -1, 1), GridSpace::Blocked),
            (virt_d3(-1, 0, 0), GridSpace::LineSegment { axis: Axis::UnDs, hardened: false }),
            (virt_d3(-1, 0, 1), GridSpace::Blocked),
            (virt_d3(0, -1, 0), GridSpace::LineSegment { axis: Axis::NwSe, hardened: true }),
        ],
        vec![
            smove(virt_d3(0, 0, 0), Direction::NorthEast),
            smove(virt_d3(0, 0, 0), Direction::NorthWest),
            smove(virt_d3(0, 0, 0), Direction::SouthEast),
            smove(virt_d3(0, 0, 0), Direction::SouthWest),
            smove(virt_d3(0, 0, 0), Direction::UpNorth),
            smove(virt_d3(0, 0, 0), Direction::UpSouth),
            smove(virt_d3(0, 0, 0), Direction::UpEast),
        ]
    )]
    fn spotcheck_move_generation(
        mut empty_grid: Grid,
        #[case] player: SpicePlayer,
        #[case] positions_to_fill: Vec<(VirtD3, GridSpace)>,
        #[case] mut expected_moves: Vec<SpiceMove>,
    ) {
        for (index, value) in positions_to_fill {
            empty_grid.set_vc_unchecked(index, value);
        }

        let mut actual_moves = generate_moves(&empty_grid, player);

        sort_move_list(&mut actual_moves);
        sort_move_list(&mut expected_moves);

        assert_eq!(actual_moves, expected_moves);
    }

    // TODO: test apply_move and associated subroutines
}
