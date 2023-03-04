use mcts::GameState;

use super::{coord::*, direction::*, grid::*, players::*, SpiceState};

#[derive(Debug, PartialEq, Eq, Clone)]
pub struct SpiceMove {
    source: VirtD3,
    direction: Direction,
}

impl Default for SpiceMove {
    fn default() -> Self {
        Self {
            source: virt_d3(0, 0, 0),
            direction: Direction::DownEast,
        }
    }
}

#[derive(Debug, PartialEq, Eq, Clone, Default)]
pub struct MoveCache {
    blue_endpoint_coords: Vec<VirtD3>,
    red_endpoint_coords: Vec<VirtD3>,
}

impl MoveCache {
    pub fn from_grid(grid: &Grid) -> Self {
        let mut rval: Self = Default::default();

        for (c, s) in grid.enumerate_vc() {
            if let GridSpace::Endpoint { owner, .. } = s {
                rval.add_endpoint(*owner, c);
            }
        }

        rval
    }

    fn add_endpoint(&mut self, player: SpicePlayer, coord: VirtD3) {
        self._endpoint_coords_mut(player).push(coord);
    }

    fn remove_endpoint(&mut self, player: SpicePlayer, coord: VirtD3) {
        self._endpoint_coords_mut(player).retain(|&c| c != coord);
    }

    fn endpoint_coords(&self, player: SpicePlayer) -> &Vec<VirtD3> {
        match player {
            SpicePlayer::Blue => &self.blue_endpoint_coords,
            SpicePlayer::Red => &self.red_endpoint_coords,
        }
    }

    fn _endpoint_coords_mut(&mut self, player: SpicePlayer) -> &mut Vec<VirtD3> {
        match player {
            SpicePlayer::Blue => &mut self.blue_endpoint_coords,
            SpicePlayer::Red => &mut self.red_endpoint_coords,
        }
    }
}

pub fn generate_moves(grid: &Grid, player: SpicePlayer, move_cache: &MoveCache) -> Vec<SpiceMove> {
    let coords = move_cache.endpoint_coords(player);
    let mut moves: Vec<SpiceMove> = Vec::with_capacity(coords.len() * 12);

    for &c in coords {
        for d in Direction::ALL {
            if grid.is_valid_and_empty_vc(c + d) {
                moves.push(SpiceMove {
                    source: c,
                    direction: d,
                });
            }
        }
    }

    moves
}

pub fn out_of_moves(grid: &Grid, player: SpicePlayer, move_cache: &MoveCache) -> bool {
    for &c in move_cache.endpoint_coords(player) {
        for d in Direction::ALL {
            if grid.is_valid_and_empty_vc(c + d) {
                return false;
            }
        }
    }

    true
}

pub fn apply_move(
    grid: &mut Grid,
    move_cache: &mut MoveCache,
    move_: &SpiceMove,
    player: SpicePlayer,
) {
    // TODO: use anyhow instead of immediately panicking?

    update_start_endpoint(grid, move_.source);

    let axis = move_.direction.axis();
    let mut ray_coord = move_.source + move_.direction;
    while let Some(space) = grid.get_mut_vc(ray_coord) {
        match space {
            GridSpace::Empty => {
                *space = GridSpace::LineSegment {
                    axis,
                    hardened: false,
                };
            }

            GridSpace::LineSegment { axis: ax, hardened } => {
                if *hardened {
                    break;
                }

                let (dir1, dir2) = ax.directions();

                *space = GridSpace::LineSegment {
                    axis,
                    hardened: true,
                };

                cut_line_in_direction(grid, move_cache, ray_coord + dir1, dir1);
                cut_line_in_direction(grid, move_cache, ray_coord + dir2, dir2);
            }

            GridSpace::Blocked | GridSpace::Endpoint { .. } => break,
        }

        ray_coord += move_.direction;
    }

    let end_coord = ray_coord - move_.direction;
    create_end_endpoint(grid, end_coord, player);
    move_cache.add_endpoint(player, end_coord);
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

fn cut_line_in_direction(
    grid: &mut Grid,
    move_cache: &mut MoveCache,
    start_coord: VirtD3,
    dir: Direction,
) {
    let mut coord = start_coord;
    while let Some(space) = grid.get_mut_vc(coord) {
        match space {
            GridSpace::LineSegment { .. } => {
                *space = GridSpace::Empty;
            }

            GridSpace::Endpoint {
                owner,
                connected_lines,
            } => {
                if *connected_lines > 1 {
                    *connected_lines -= 1;
                } else {
                    move_cache.remove_endpoint(*owner, coord);
                    *space = GridSpace::Blocked;
                };

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

        let move_cache = MoveCache::from_grid(&empty_grid);
        let mut actual_moves = generate_moves(&empty_grid, player, &move_cache);

        sort_move_list(&mut actual_moves);
        sort_move_list(&mut expected_moves);

        assert_eq!(actual_moves, expected_moves);
    }

    #[rstest]
    fn apply_move_basic_functionality(mut empty_grid: Grid) {
        let player = SpicePlayer::Blue;
        let start_pos = virt_d3(3, 3, 3);
        let end_pos = virt_d3(-3, 3, 3);
        let dir = Direction::DownSouth;

        empty_grid.set_vc_unchecked(
            start_pos,
            GridSpace::Endpoint {
                owner: player,
                connected_lines: 0,
            },
        );
        let move_ = SpiceMove {
            source: start_pos,
            direction: dir,
        };
        let mut move_cache = MoveCache::from_grid(&empty_grid);

        apply_move(&mut empty_grid, &mut move_cache, &move_, SpicePlayer::Blue);

        assert_eq!(
            empty_grid.get_vc(start_pos),
            Some(&GridSpace::Endpoint {
                owner: player,
                connected_lines: 1
            }),
            "start space has the wrong values"
        );

        assert_eq!(
            empty_grid.get_vc(end_pos),
            Some(&GridSpace::Endpoint {
                owner: player,
                connected_lines: 1
            }),
            "end space has the wrong values"
        );

        let mut line_coord = start_pos + dir;
        let axis = dir.axis();
        while line_coord != end_pos {
            assert_eq!(
                empty_grid.get_vc(line_coord),
                Some(&GridSpace::LineSegment {
                    axis,
                    hardened: false
                }),
                "space at {line_coord} has the wrong values"
            );

            line_coord += dir;
        }
    }

    #[rstest]
    fn apply_move_cuts_existing_line(mut empty_grid: Grid) {
        let move_player = SpicePlayer::Blue;
        let line_player = SpicePlayer::Blue;
        let move_source = virt_d3(3, 3, 3);
        let move_dir = Direction::DownSouth;
        let line_pos = virt_d3(0, 3, 3);
        let line_axis = Axis::NeSw;
        let line_dirs = line_axis.directions();

        empty_grid.set_vc_unchecked(
            move_source,
            GridSpace::Endpoint {
                owner: move_player,
                connected_lines: 0,
            },
        );
        empty_grid.set_vc_unchecked(
            line_pos,
            GridSpace::LineSegment {
                axis: line_axis,
                hardened: false,
            },
        );
        empty_grid.set_vc_unchecked(
            line_pos + line_dirs.0,
            GridSpace::Endpoint {
                owner: line_player,
                connected_lines: 1,
            },
        );
        empty_grid.set_vc_unchecked(
            line_pos + line_dirs.1,
            GridSpace::Endpoint {
                owner: line_player,
                connected_lines: 1,
            },
        );
        let move_ = SpiceMove {
            source: move_source,
            direction: move_dir,
        };
        let mut move_cache = MoveCache::from_grid(&empty_grid);

        apply_move(&mut empty_grid, &mut move_cache, &move_, move_player);

        assert_eq!(
            empty_grid.get_vc(line_pos),
            Some(&GridSpace::LineSegment {
                axis: move_dir.axis(),
                hardened: true
            }),
            "space at the cut point has the wrong values"
        );

        assert_eq!(
            empty_grid.get_vc(line_pos + line_dirs.0),
            Some(&GridSpace::Blocked),
            "first space adjacent to the cut point has the wrong values"
        );

        assert_eq!(
            empty_grid.get_vc(line_pos + line_dirs.1),
            Some(&GridSpace::Blocked),
            "second space adjacent to the cut point has the wrong values"
        );
    }

    #[rstest]
    #[case(GridSpace::Blocked)]
    #[case(GridSpace::LineSegment { axis: Axis::NeSw, hardened: true })]
    #[case(GridSpace::Endpoint { owner: SpicePlayer::Red, connected_lines: 1 })]
    fn apply_move_stops_at_blocked_spaces(mut empty_grid: Grid, #[case] blocker: GridSpace) {
        let source_pos = virt_d3(-3, -3, -3);
        let player = SpicePlayer::Red;
        let dir = Direction::UpNorth;
        let blocker_pos = virt_d3(0, -3, -3);

        empty_grid.set_vc_unchecked(
            source_pos,
            GridSpace::Endpoint {
                owner: player,
                connected_lines: 0,
            },
        );
        empty_grid.set_vc_unchecked(blocker_pos, blocker);
        let move_ = SpiceMove {
            source: source_pos,
            direction: dir,
        };
        let mut move_cache = MoveCache::from_grid(&empty_grid);

        apply_move(&mut empty_grid, &mut move_cache, &move_, player);

        assert_eq!(
            empty_grid.get_vc(blocker_pos - dir),
            Some(&GridSpace::Endpoint {
                owner: player,
                connected_lines: 1
            }),
            "space before blocker has the wrong values"
        );
    }
}
