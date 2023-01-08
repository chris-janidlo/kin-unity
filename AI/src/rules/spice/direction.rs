use std::ops::{Add, AddAssign, Sub};

use glam::*;
use rand::{
    distributions::{Distribution, Standard},
    Rng,
};

#[derive(Debug, PartialEq, Eq, Clone, Copy)]
pub enum Direction {
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest,
    UpNorth,
    UpSouth,
    UpEast,
    UpWest,
    DownNorth,
    DownSouth,
    DownEast,
    DownWest,
}

#[derive(Debug, PartialEq, Eq, Clone, Copy)]
pub enum Axis {
    NeSw,
    NwSe,
    UnDs,
    UsDn,
    UeDw,
    UwDe,
}

impl From<Direction> for IVec3 {
    fn from(value: Direction) -> Self {
        match value {
            Direction::NorthEast => ivec3(1, 1, 0),
            Direction::NorthWest => ivec3(-1, 1, 0),
            Direction::SouthEast => ivec3(1, -1, 0),
            Direction::SouthWest => ivec3(-1, -1, 0),
            Direction::UpNorth => ivec3(0, 1, 1),
            Direction::UpSouth => ivec3(0, -1, 1),
            Direction::UpEast => ivec3(1, 0, 1),
            Direction::UpWest => ivec3(-1, 0, 1),
            Direction::DownNorth => ivec3(0, 1, -1),
            Direction::DownSouth => ivec3(0, -1, -1),
            Direction::DownEast => ivec3(1, 0, -1),
            Direction::DownWest => ivec3(-1, 0, -1),
        }
    }
}

impl Add<Direction> for IVec3 {
    type Output = IVec3;

    fn add(self, rhs: Direction) -> Self::Output {
        self + IVec3::from(rhs)
    }
}

impl Add<IVec3> for Direction {
    type Output = IVec3;

    fn add(self, rhs: IVec3) -> Self::Output {
        IVec3::from(self) + rhs
    }
}

impl AddAssign<Direction> for IVec3 {
    fn add_assign(&mut self, rhs: Direction) {
        *self = *self + rhs
    }
}

impl Sub<Direction> for IVec3 {
    type Output = IVec3;

    fn sub(self, rhs: Direction) -> Self::Output {
        self - IVec3::from(rhs)
    }
}

impl Distribution<Direction> for Standard {
    fn sample<R: Rng + ?Sized>(&self, rng: &mut R) -> Direction {
        match rng.gen_range(0..12) {
            0 => Direction::NorthEast,
            1 => Direction::NorthWest,
            2 => Direction::SouthEast,
            3 => Direction::SouthWest,
            4 => Direction::UpNorth,
            5 => Direction::UpSouth,
            6 => Direction::UpEast,
            7 => Direction::UpWest,
            8 => Direction::DownNorth,
            9 => Direction::DownSouth,
            10 => Direction::DownEast,
            11 => Direction::DownWest,
            _ => unreachable!(),
        }
    }
}

impl Axis {
    pub fn directions(self) -> (Direction, Direction) {
        match self {
            Axis::NeSw => (Direction::NorthEast, Direction::SouthWest),
            Axis::NwSe => (Direction::NorthWest, Direction::SouthEast),
            Axis::UnDs => (Direction::UpNorth, Direction::DownSouth),
            Axis::UsDn => (Direction::UpSouth, Direction::DownNorth),
            Axis::UeDw => (Direction::UpEast, Direction::DownWest),
            Axis::UwDe => (Direction::UpWest, Direction::DownEast),
        }
    }

    pub fn vectors(self) -> (IVec3, IVec3) {
        let dirs = self.directions();
        (dirs.0.into(), dirs.1.into())
    }
}

impl Direction {
    pub fn axis(self) -> Axis {
        match self {
            Direction::NorthWest | Direction::SouthEast => Axis::NwSe,
            Direction::NorthEast | Direction::SouthWest => Axis::NeSw,
            Direction::UpNorth | Direction::DownSouth => Axis::UnDs,
            Direction::UpSouth | Direction::DownNorth => Axis::UsDn,
            Direction::UpEast | Direction::DownWest => Axis::UeDw,
            Direction::UpWest | Direction::DownEast => Axis::UwDe,
        }
    }
}
