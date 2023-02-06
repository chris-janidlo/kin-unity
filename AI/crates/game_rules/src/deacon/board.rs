use std::ops::Mul;

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

#[derive(Debug, PartialEq, Eq, Clone, Hash)]
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
        &self.positions[coord.x as usize][coord.y as usize]
    }

    pub fn set(&mut self, coord: Coordinate, value: Option<Piece>) {
        self.positions[coord.x as usize][coord.y as usize] = value;
    }

    pub fn enumer_coords(&self) -> impl Iterator<Item = (Coordinate, &Option<Piece>)> {
        self.positions.iter().enumerate().flat_map(|(x, row)| {
            row.iter().enumerate().map(move |(y, p)| {
                let x = x.try_into().unwrap();
                let y = y.try_into().unwrap();
                (Coordinate { x, y }, p)
            })
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
}
