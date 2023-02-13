use std::ops::*;

use super::coord::*;

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

    pub fn vectors_real(self) -> (Real, Real) {
        let dirs = self.directions();
        (dirs.0.into(), dirs.1.into())
    }

    pub fn vectors_virt_d3(self) -> (VirtD3, VirtD3) {
        let dirs = self.directions();
        (dirs.0.into(), dirs.1.into())
    }
}

impl From<Direction> for VirtD3 {
    fn from(value: Direction) -> Self {
        match value {
            Direction::NorthEast => virt_d3(0, 0, 1),
            Direction::NorthWest => virt_d3(1, -1, 0),
            Direction::SouthEast => virt_d3(-1, 1, 0),
            Direction::SouthWest => virt_d3(0, 0, -1),
            Direction::UpNorth => virt_d3(1, 0, 0),
            Direction::UpSouth => virt_d3(0, 1, -1),
            Direction::UpEast => virt_d3(0, 1, 0),
            Direction::UpWest => virt_d3(1, 0, -1),
            Direction::DownNorth => virt_d3(0, -1, 1),
            Direction::DownSouth => virt_d3(-1, 0, 0),
            Direction::DownEast => virt_d3(-1, 0, 1),
            Direction::DownWest => virt_d3(0, -1, 0),
        }
    }
}

impl From<Direction> for Real {
    fn from(value: Direction) -> Self {
        Real::from(VirtD3::from(value))
    }
}

impl Neg for Direction {
    type Output = Self;

    fn neg(self) -> Self::Output {
        match self {
            Direction::NorthEast => Direction::SouthWest,
            Direction::NorthWest => Direction::SouthEast,
            Direction::SouthEast => Direction::NorthWest,
            Direction::SouthWest => Direction::NorthEast,
            Direction::UpNorth => Direction::DownSouth,
            Direction::UpSouth => Direction::DownNorth,
            Direction::UpEast => Direction::DownWest,
            Direction::UpWest => Direction::DownEast,
            Direction::DownNorth => Direction::UpSouth,
            Direction::DownSouth => Direction::UpNorth,
            Direction::DownEast => Direction::UpWest,
            Direction::DownWest => Direction::UpEast,
        }
    }
}

impl Add<Direction> for Real {
    type Output = Real;

    fn add(self, rhs: Direction) -> Self::Output {
        self + Real::from(rhs)
    }
}

impl Add<Real> for Direction {
    type Output = Real;

    fn add(self, rhs: Real) -> Self::Output {
        Real::from(self) + rhs
    }
}

impl AddAssign<Direction> for Real {
    fn add_assign(&mut self, rhs: Direction) {
        *self = *self + rhs
    }
}

impl Sub<Direction> for Real {
    type Output = Real;

    fn sub(self, rhs: Direction) -> Self::Output {
        self + -rhs
    }
}

impl SubAssign<Direction> for Real {
    fn sub_assign(&mut self, rhs: Direction) {
        *self = *self - rhs
    }
}

impl Add<Direction> for VirtD3 {
    type Output = VirtD3;

    fn add(self, rhs: Direction) -> Self::Output {
        self + VirtD3::from(rhs)
    }
}

impl Add<VirtD3> for Direction {
    type Output = VirtD3;

    fn add(self, rhs: VirtD3) -> Self::Output {
        VirtD3::from(self) + rhs
    }
}

impl AddAssign<Direction> for VirtD3 {
    fn add_assign(&mut self, rhs: Direction) {
        *self = *self + rhs
    }
}

impl Sub<Direction> for VirtD3 {
    type Output = VirtD3;

    fn sub(self, rhs: Direction) -> Self::Output {
        self + -rhs
    }
}

impl SubAssign<Direction> for VirtD3 {
    fn sub_assign(&mut self, rhs: Direction) {
        *self = *self - rhs
    }
}

#[cfg(test)]
mod tests {
    use rand::prelude::*;
    use rstest::*;

    use super::*;

    #[fixture]
    fn all_directions() -> [Direction; 12] {
        [
            Direction::NorthEast,
            Direction::NorthWest,
            Direction::SouthEast,
            Direction::SouthWest,
            Direction::UpNorth,
            Direction::UpSouth,
            Direction::UpEast,
            Direction::UpWest,
            Direction::DownNorth,
            Direction::DownSouth,
            Direction::DownEast,
            Direction::DownWest,
        ]
    }

    #[fixture]
    fn all_axes() -> [Axis; 6] {
        [
            Axis::NeSw,
            Axis::NwSe,
            Axis::UnDs,
            Axis::UsDn,
            Axis::UeDw,
            Axis::UwDe,
        ]
    }

    #[rstest]
    fn direction_double_neg_is_noop(all_directions: [Direction; 12]) {
        for dir in all_directions {
            assert_eq!(dir, -(-dir));
        }
    }

    #[rstest]
    fn direction_minus_self_is_zero(all_directions: [Direction; 12]) {
        for dir in all_directions {
            let r: Real = dir.into();
            let v: VirtD3 = dir.into();

            assert_eq!(r - dir, real(0, 0, 0));
            assert_eq!(v - dir, virt_d3(0, 0, 0));
        }
    }

    #[rstest]
    fn real_virt_conversions_are_equivalent(all_directions: [Direction; 12]) {
        for dir in all_directions {
            let real: Real = dir.into();
            let virt: VirtD3 = dir.into();

            println!("dir: {dir:?}: {real}, {virt}");
            assert_eq!(real, virt.into());
            assert_eq!(virt, real.try_into().unwrap())
        }
    }

    #[rstest]
    fn axis_directions_are_negatives(all_axes: [Axis; 6]) {
        for axis in all_axes {
            let (dir1, dir2) = axis.directions();
            assert_eq!(dir1, -dir2);
            assert_eq!(-dir1, dir2);
        }
    }
}
