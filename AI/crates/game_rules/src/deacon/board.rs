use std::ops::Mul;

use itertools::*;

pub use super::{form::Form, Piece, Player};

macro_rules! board {
    [ $([ $($i:tt),+ ]);* $(;)? ] => {
        Board {
            positions: [ $(
                [ $( board!($i), )+ ],
            )* ]
        }
    };
    //
    (_) => { None::<Piece> };
    //
    (C) => { Some(Piece { form: Form::Captain, owner: Player::Blue }) };
    (c) => { Some(Piece { form: Form::Captain, owner: Player::Red }) };
    //
    (E) => { Some(Piece { form: Form::Engineer, owner: Player::Blue, }) };
    (e) => { Some(Piece { form: Form::Engineer, owner: Player::Red, }) };
    //
    (I) => { Some(Piece { form: Form::Pilot, owner: Player::Blue, }) };
    (i) => { Some(Piece { form: Form::Pilot, owner: Player::Red, }) };
    //
    (P) => { Some(Piece { form: Form::Priest, owner: Player::Blue, }) };
    (p) => { Some(Piece { form: Form::Priest, owner: Player::Red, }) };
    //
    (R) => { Some(Piece { form: Form::Robot, owner: Player::Blue, }) };
    (r) => { Some(Piece { form: Form::Robot, owner: Player::Red, }) };
    //
    (S) => { Some(Piece { form: Form::Scientist, owner: Player::Blue, }) };
    (s) => { Some(Piece { form: Form::Scientist, owner: Player::Red, }) };
}

pub(crate) use board;

pub const BOARD_LENGTH: usize = 5;

#[derive(Debug, PartialEq, Eq, Clone, Hash, PartialOrd, Ord)]
pub struct Board {
    pub positions: [[Option<Piece>; BOARD_LENGTH]; BOARD_LENGTH],
}

#[repr(u8)]
#[derive(Debug, PartialEq, Eq, Clone, Copy, PartialOrd, Ord)]
pub enum CoordCmpnt {
    Zero,
    One,
    Two,
    Three,
    Four,
}

#[derive(Debug, PartialEq, Eq, Clone, Copy, PartialOrd, Ord)]
pub struct Coordinate {
    pub x: CoordCmpnt,
    pub y: CoordCmpnt,
}

#[derive(Debug, PartialEq, Eq, Clone, Copy)]
pub struct RelCoord {
    pub x: i8,
    pub y: i8,
}

pub fn rcoord(x: i8, y: i8) -> RelCoord {
    RelCoord { x, y }
}

#[cfg(test)] // currently only used in tests, but may be useful later outside of them
pub fn coord_unwrapped(x: u8, y: u8) -> Coordinate {
    Coordinate {
        x: x.try_into().unwrap(),
        y: y.try_into().unwrap(),
    }
}

impl Default for Board {
    fn default() -> Self {
        board![
            [_, S, _, _, _];
            [S, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, s];
            [_, _, _, s, _];
        ]
    }
}

impl Board {
    pub fn get(&self, coord: Coordinate) -> &Option<Piece> {
        &self.positions[coord.y as usize][coord.x as usize]
    }

    pub fn set(&mut self, coord: Coordinate, value: Option<Piece>) {
        self.positions[coord.y as usize][coord.x as usize] = value;
    }

    pub fn enumer_coords(&self) -> impl Iterator<Item = (Coordinate, &Option<Piece>)> {
        iproduct!(0..BOARD_LENGTH, 0..BOARD_LENGTH).map(|(x, y)| {
            let x = x.try_into().unwrap();
            let y = y.try_into().unwrap();
            let coord = Coordinate { x, y };
            let piece = self.get(coord);

            (coord, piece)
        })
    }

    pub fn pieces(&self) -> impl Iterator<Item = &Piece> {
        self.positions.iter().flatten().filter_map(|x| x.as_ref())
    }
}

impl From<CoordCmpnt> for i8 {
    fn from(value: CoordCmpnt) -> Self {
        match value {
            CoordCmpnt::Zero => 0,
            CoordCmpnt::One => 1,
            CoordCmpnt::Two => 2,
            CoordCmpnt::Three => 3,
            CoordCmpnt::Four => 4,
        }
    }
}

impl TryFrom<usize> for CoordCmpnt {
    type Error = &'static str;

    fn try_from(value: usize) -> Result<Self, Self::Error> {
        match value {
            0 => Ok(CoordCmpnt::Zero),
            1 => Ok(CoordCmpnt::One),
            2 => Ok(CoordCmpnt::Two),
            3 => Ok(CoordCmpnt::Three),
            4 => Ok(CoordCmpnt::Four),
            _ => Err("CoordCmpnt must be less than 5"),
        }
    }
}

impl TryFrom<u8> for CoordCmpnt {
    type Error = &'static str;

    fn try_from(value: u8) -> Result<Self, Self::Error> {
        match value {
            0 => Ok(CoordCmpnt::Zero),
            1 => Ok(CoordCmpnt::One),
            2 => Ok(CoordCmpnt::Two),
            3 => Ok(CoordCmpnt::Three),
            4 => Ok(CoordCmpnt::Four),
            _ => Err("CoordCmpnt must be less than 5"),
        }
    }
}

impl TryFrom<i8> for CoordCmpnt {
    type Error = &'static str;

    fn try_from(value: i8) -> Result<Self, Self::Error> {
        match value {
            0 => Ok(CoordCmpnt::Zero),
            1 => Ok(CoordCmpnt::One),
            2 => Ok(CoordCmpnt::Two),
            3 => Ok(CoordCmpnt::Three),
            4 => Ok(CoordCmpnt::Four),
            _ => Err("value must be in range [0, 4]"),
        }
    }
}

impl Coordinate {
    pub fn checked_add(self, other: RelCoord) -> Result<Self, &'static str> {
        let x: CoordCmpnt = (i8::from(self.x) + other.x).try_into()?;
        let y: CoordCmpnt = (i8::from(self.y) + other.y).try_into()?;

        Ok(Self { x, y })
    }
}

impl Mul<i8> for RelCoord {
    type Output = RelCoord;

    fn mul(self, rhs: i8) -> Self::Output {
        Self::Output {
            x: self.x * rhs,
            y: self.y * rhs,
        }
    }
}

#[cfg(test)]
mod tests {
    use pretty_assertions::assert_eq;
    use rand::prelude::*;
    use rstest::*;

    use super::*;

    #[rstest]
    fn verify_board_macro() {
        let board_macro = board![
            [C, c, E, e, _];
            [I, i, P, p, _];
            [R, r, _, _, _];
            [_, _, S, s, _];
            [_, _, _, _, _];
        ];

        let board_struct_expression = Board {
            positions: [
                [
                    Some(Piece {
                        form: Form::Captain,
                        owner: Player::Blue,
                    }),
                    Some(Piece {
                        form: Form::Captain,
                        owner: Player::Red,
                    }),
                    Some(Piece {
                        form: Form::Engineer,
                        owner: Player::Blue,
                    }),
                    Some(Piece {
                        form: Form::Engineer,
                        owner: Player::Red,
                    }),
                    None,
                ],
                [
                    Some(Piece {
                        form: Form::Pilot,
                        owner: Player::Blue,
                    }),
                    Some(Piece {
                        form: Form::Pilot,
                        owner: Player::Red,
                    }),
                    Some(Piece {
                        form: Form::Priest,
                        owner: Player::Blue,
                    }),
                    Some(Piece {
                        form: Form::Priest,
                        owner: Player::Red,
                    }),
                    None,
                ],
                [
                    Some(Piece {
                        form: Form::Robot,
                        owner: Player::Blue,
                    }),
                    Some(Piece {
                        form: Form::Robot,
                        owner: Player::Red,
                    }),
                    None,
                    None,
                    None,
                ],
                [
                    None,
                    None,
                    Some(Piece {
                        form: Form::Scientist,
                        owner: Player::Blue,
                    }),
                    Some(Piece {
                        form: Form::Scientist,
                        owner: Player::Red,
                    }),
                    None,
                ],
                [None, None, None, None, None],
            ],
        };

        assert_eq!(board_macro, board_struct_expression);
    }

    #[rstest]
    #[case::empty_board(
        board![
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
        ],
        vec![
            (coord_unwrapped(0, 0), None::<Piece>),
            (coord_unwrapped(0, 1), None::<Piece>),
            (coord_unwrapped(0, 2), None::<Piece>),
            (coord_unwrapped(0, 3), None::<Piece>),
            (coord_unwrapped(0, 4), None::<Piece>),
            (coord_unwrapped(1, 0), None::<Piece>),
            (coord_unwrapped(1, 1), None::<Piece>),
            (coord_unwrapped(1, 2), None::<Piece>),
            (coord_unwrapped(1, 3), None::<Piece>),
            (coord_unwrapped(1, 4), None::<Piece>),
            (coord_unwrapped(2, 0), None::<Piece>),
            (coord_unwrapped(2, 1), None::<Piece>),
            (coord_unwrapped(2, 2), None::<Piece>),
            (coord_unwrapped(2, 3), None::<Piece>),
            (coord_unwrapped(2, 4), None::<Piece>),
            (coord_unwrapped(3, 0), None::<Piece>),
            (coord_unwrapped(3, 1), None::<Piece>),
            (coord_unwrapped(3, 2), None::<Piece>),
            (coord_unwrapped(3, 3), None::<Piece>),
            (coord_unwrapped(3, 4), None::<Piece>),
            (coord_unwrapped(4, 0), None::<Piece>),
            (coord_unwrapped(4, 1), None::<Piece>),
            (coord_unwrapped(4, 2), None::<Piece>),
            (coord_unwrapped(4, 3), None::<Piece>),
            (coord_unwrapped(4, 4), None::<Piece>),
        ]
    )]
    #[case::coordinate_ordering(
        board![
            [_, S, _, _, _];
            [_, _, _, _, _];
            [S, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
        ],
        vec![
            (coord_unwrapped(0, 0), None::<Piece>),
            (coord_unwrapped(0, 1), None::<Piece>),
            (coord_unwrapped(0, 2), Some(Piece { form: Form::Scientist, owner: Player::Blue })),
            (coord_unwrapped(0, 3), None::<Piece>),
            (coord_unwrapped(0, 4), None::<Piece>),
            (coord_unwrapped(1, 0), Some(Piece { form: Form::Scientist, owner: Player::Blue })),
            (coord_unwrapped(1, 1), None::<Piece>),
            (coord_unwrapped(1, 2), None::<Piece>),
            (coord_unwrapped(1, 3), None::<Piece>),
            (coord_unwrapped(1, 4), None::<Piece>),
            (coord_unwrapped(2, 0), None::<Piece>),
            (coord_unwrapped(2, 1), None::<Piece>),
            (coord_unwrapped(2, 2), None::<Piece>),
            (coord_unwrapped(2, 3), None::<Piece>),
            (coord_unwrapped(2, 4), None::<Piece>),
            (coord_unwrapped(3, 0), None::<Piece>),
            (coord_unwrapped(3, 1), None::<Piece>),
            (coord_unwrapped(3, 2), None::<Piece>),
            (coord_unwrapped(3, 3), None::<Piece>),
            (coord_unwrapped(3, 4), None::<Piece>),
            (coord_unwrapped(4, 0), None::<Piece>),
            (coord_unwrapped(4, 1), None::<Piece>),
            (coord_unwrapped(4, 2), None::<Piece>),
            (coord_unwrapped(4, 3), None::<Piece>),
            (coord_unwrapped(4, 4), None::<Piece>),
        ]
    )]
    fn test_enumer_coords(
        #[case] board: Board,
        #[case] expected_enumeration: Vec<(Coordinate, Option<Piece>)>,
    ) {
        let actual_enumeration = board
            .enumer_coords()
            .map(|(c, p)| (c, p.to_owned()))
            .collect_vec();

        assert_eq!(actual_enumeration, expected_enumeration);
    }

    #[rstest]
    #[case::empty(
        board![
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
        ],
        coord_unwrapped(0, 0),
        None
    )]
    #[case::non_empty(
        board![
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, E, _, _, _];
            [_, _, _, _, _];
        ],
        coord_unwrapped(1, 3),
        Some(Piece { form: Form::Engineer, owner: Player::Blue })
    )]
    fn test_get_position(
        #[case] board: Board,
        #[case] coord: Coordinate,
        #[case] expected_piece: Option<Piece>,
    ) {
        let actual_piece = board.get(coord).to_owned();

        assert_eq!(actual_piece, expected_piece);
    }

    #[rstest]
    #[case::last_corner_from_empty(
        board![
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
        ],
        coord_unwrapped(4, 4),
        Some(Piece { form: Form::Pilot, owner: Player::Red }),
        board![
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, i];
        ],
    )]
    #[case::proper_ordering(
        board![
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
        ],
        coord_unwrapped(2, 3),
        Some(Piece { form: Form::Priest, owner: Player::Red }),
        board![
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, p, _, _];
            [_, _, _, _, _];
        ],
    )]
    #[case::overwrite(
        board![
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, P, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
        ],
        coord_unwrapped(2, 2),
        Some(Piece { form: Form::Captain, owner: Player::Red }),
        board![
            [_, _, _, _, _];
            [_, _, _, _, _];
            [_, _, c, _, _];
            [_, _, _, _, _];
            [_, _, _, _, _];
        ],
    )]
    fn test_set_position(
        #[case] mut start_board: Board,
        #[case] coord: Coordinate,
        #[case] piece: Option<Piece>,
        #[case] expected_board: Board,
    ) {
        start_board.set(coord, piece);

        assert_eq!(start_board, expected_board);
    }

    #[rstest]
    fn test_set_position_get_position_parity() {
        fn random_board() -> Board {
            let mut board = Board::default();

            for (x, y) in iproduct!(0..BOARD_LENGTH, 0..BOARD_LENGTH) {
                board.positions[x][y] = random_piece();
            }

            board
        }

        fn random_piece() -> Option<Piece> {
            let r: u8 = rand::random();

            if r > 127 {
                None
            } else {
                let owner = if r % 2 == 0 {
                    Player::Blue
                } else {
                    Player::Red
                };

                let form = match r {
                    // 128 / 6 ~= 21
                    _ if r < 21 => Form::Captain,
                    _ if r < 42 => Form::Engineer,
                    _ if r < 63 => Form::Pilot,
                    _ if r < 84 => Form::Priest,
                    _ if r < 105 => Form::Robot,
                    _ => Form::Scientist,
                };

                Some(Piece { form, owner })
            }
        }

        fn random_coord() -> Coordinate {
            let x = rand::thread_rng().gen_range(0..BOARD_LENGTH);
            let y = rand::thread_rng().gen_range(0..BOARD_LENGTH);

            coord_unwrapped(x as u8, y as u8)
        }

        for _ in 0..100 {
            let start_board: Board = random_board();
            let piece: Option<Piece> = random_piece();
            let coord: Coordinate = random_coord();

            let mut new_board = start_board.clone();
            new_board.set(coord, piece.clone());

            let result = new_board.get(coord).to_owned();

            assert_eq!(
                piece, result,
                "\n\t* start_board: {start_board:?}\n\t* coord:{coord:?}\n\t* new_board: {new_board:?}"
            );
        }
    }
}
