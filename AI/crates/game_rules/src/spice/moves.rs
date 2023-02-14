use mcts::GameState;

use super::{coord::*, direction::*, grid::*, SpiceState};

#[derive(Debug, PartialEq, Eq)]
pub struct SpiceMove {
    source: VirtD3,
    direction: Direction,
}

pub struct SpiceMoveIterator {
    moves: Vec<SpiceMove>,
}

impl SpiceMoveIterator {
    pub fn new(state: &SpiceState) -> Self {
        // TODO: can we do the move generation lazily? the type of `moves` without the
        // collect is a mess of i: dilosures, which seems hard to unravel (to see for
        // yourself, remove the .collect() call on the moves iter and run cargo check),
        // not to mention the lifetime on the state's grid we'd have to somehow track

        let owned_endpoint_coords = state.grid.enumerate_vc().filter_map(|(c, s)| match s {
            GridSpace::Endpoint { owner, .. } if *owner == state.next_to_play() => Some(c),
            _ => None,
        });

        let moves = owned_endpoint_coords
            .flat_map(|c| {
                Direction::ALL.iter().filter_map(move |&d| {
                    state.grid.get_vc(c + d).as_ref().and_then(|s| match s {
                        GridSpace::Empty => Some(SpiceMove {
                            source: c,
                            direction: d,
                        }),
                        _ => None,
                    })
                })
            })
            .collect();

        Self { moves }
    }
}

impl Iterator for SpiceMoveIterator {
    type Item = SpiceMove;

    fn next(&mut self) -> Option<Self::Item> {
        self.moves.pop()
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
            (virt_d3(-1, 0, 0), GridSpace::Line(SpicePlayer::Red, Axis::UnDs)),
            (virt_d3(-1, 0, 1), GridSpace::Blocked),
            (virt_d3(0, -1, 0), GridSpace::Line(SpicePlayer::Blue, Axis::NwSe)),
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

        let state = SpiceState {
            grid: empty_grid,
            player,
        };

        let mut actual_moves: Vec<SpiceMove> = SpiceMoveIterator::new(&state).collect();

        sort_move_list(&mut actual_moves);
        sort_move_list(&mut expected_moves);

        assert_eq!(actual_moves, expected_moves);
    }
}
