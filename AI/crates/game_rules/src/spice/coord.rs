use std::{
    fmt::{Debug, Display},
    ops::*,
};

pub trait Coord: Sized + Copy {
    fn length_squared(self) -> f32;
    fn length(self) -> f32;
}

/// A point in Z3. May be convertable into a [VirtD3].
#[derive(PartialEq, Eq, Clone, Copy)]
pub struct Real {
    pub x: i16,
    pub y: i16,
    pub z: i16,
}

/// A virtual point with integral components which corresponds to a point in the D3 (aka
/// FCC, A3) lattice. The lattice point can be retrieved via conversion to a [Real].
#[derive(PartialEq, Eq, Clone, Copy)]
pub struct VirtD3 {
    pub i: i8,
    pub j: i8,
    pub k: i8,
}

pub fn real(x: i16, y: i16, z: i16) -> Real {
    Real { x, y, z }
}

pub fn virt_d3(i: i8, j: i8, k: i8) -> VirtD3 {
    VirtD3 { i, j, k }
}

impl From<VirtD3> for Real {
    fn from(value: VirtD3) -> Self {
        let VirtD3 { i, j, k } = value;

        let i = i as i16;
        let j = j as i16;
        let k = k as i16;

        real(j + k, i + k, i + j)
    }
}

#[derive(Debug, PartialEq, Eq)]
pub enum RealToVirtError {
    /// The [Real] value was not contained in the FCC/D3/A3 lattice.
    NotInLattice,
    /// The [Real] components were too big/small to convert.
    Saturation,
}

impl TryFrom<Real> for VirtD3 {
    type Error = RealToVirtError;

    fn try_from(value: Real) -> Result<Self, Self::Error> {
        let Real { x, y, z } = value;

        let lattice_check = x
            .checked_add(y)
            .and_then(|r| r.checked_add(z))
            .and_then(|r| r.checked_rem(2));

        match lattice_check {
            None => return Err(RealToVirtError::Saturation),
            Some(r) if r != 0 => return Err(RealToVirtError::NotInLattice),
            _ => (),
        }

        // this returns None if it overflows, underflows, or can't convert result to an i8
        fn checked_convert(p1: i16, p2: i16, n: i16) -> Option<i8> {
            // (p1 + p2 - n) / 2
            p1.checked_add(p2)
                .and_then(|a| a.checked_sub(n))
                .and_then(|s| s.checked_div(2))
                .and_then(|d| d.try_into().ok())
        }

        let vec = [
            checked_convert(y, z, x),
            checked_convert(z, x, y),
            checked_convert(x, y, z),
        ];

        if let [Some(i), Some(j), Some(k)] = vec {
            Ok(virt_d3(i, j, k))
        } else {
            Err(RealToVirtError::Saturation)
        }
    }
}

impl Coord for Real {
    fn length_squared(self) -> f32 {
        let x = self.x as f32;
        let y = self.y as f32;
        let z = self.z as f32;

        x * x + y * y + z * z
    }

    fn length(self) -> f32 {
        self.length_squared().sqrt()
    }
}

impl Coord for VirtD3 {
    fn length_squared(self) -> f32 {
        let i = self.i as f32;
        let j = self.j as f32;
        let k = self.k as f32;

        i * i + j * j + k * k
    }

    fn length(self) -> f32 {
        self.length_squared().sqrt()
    }
}

impl Add for Real {
    type Output = Self;

    fn add(self, rhs: Self) -> Self::Output {
        let Real {
            x: x1,
            y: y1,
            z: z1,
        } = self;
        let Real {
            x: x2,
            y: y2,
            z: z2,
        } = rhs;

        real(x1 + x2, y1 + y2, z1 + z2)
    }
}

impl AddAssign for Real {
    fn add_assign(&mut self, rhs: Self) {
        *self = *self + rhs;
    }
}

impl Neg for Real {
    type Output = Self;

    fn neg(self) -> Self::Output {
        let Real { x, y, z } = self;

        real(-x, -y, -z)
    }
}

impl Sub for Real {
    type Output = Self;

    fn sub(self, rhs: Self) -> Self::Output {
        self + -rhs
    }
}

impl SubAssign for Real {
    fn sub_assign(&mut self, rhs: Self) {
        *self = *self - rhs;
    }
}

impl Mul<i16> for Real {
    type Output = Self;

    fn mul(self, rhs: i16) -> Self::Output {
        let Real { x, y, z } = self;

        real(x * rhs, y * rhs, z * rhs)
    }
}

impl MulAssign<i16> for Real {
    fn mul_assign(&mut self, rhs: i16) {
        *self = *self * rhs;
    }
}

impl Display for Real {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        let Real { x, y, z } = self;
        write!(f, "real({x}, {y}, {z})")
    }
}

impl Debug for Real {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{self}")
    }
}

impl Add for VirtD3 {
    type Output = Self;

    fn add(self, rhs: Self) -> Self::Output {
        let VirtD3 {
            i: i1,
            j: j1,
            k: k1,
        } = self;
        let VirtD3 {
            i: i2,
            j: j2,
            k: k2,
        } = rhs;

        virt_d3(i1 + i2, j1 + j2, k1 + k2)
    }
}

impl AddAssign for VirtD3 {
    fn add_assign(&mut self, rhs: Self) {
        *self = *self + rhs;
    }
}

impl Neg for VirtD3 {
    type Output = Self;

    fn neg(self) -> Self::Output {
        let VirtD3 { i, j, k } = self;

        virt_d3(-i, -j, -k)
    }
}

impl Sub for VirtD3 {
    type Output = Self;

    fn sub(self, rhs: Self) -> Self::Output {
        self + -rhs
    }
}

impl SubAssign for VirtD3 {
    fn sub_assign(&mut self, rhs: Self) {
        *self = *self - rhs;
    }
}

impl Mul<i8> for VirtD3 {
    type Output = Self;

    fn mul(self, rhs: i8) -> Self::Output {
        let VirtD3 { i, j, k } = self;

        virt_d3(i * rhs, j * rhs, k * rhs)
    }
}

impl MulAssign<i8> for VirtD3 {
    fn mul_assign(&mut self, rhs: i8) {
        *self = *self * rhs;
    }
}

impl Display for VirtD3 {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        let VirtD3 { i, j, k } = self;
        write!(f, "virt({i}, {j}, {k})")
    }
}

impl Debug for VirtD3 {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "{self}")
    }
}

#[cfg(test)]
mod tests {
    use rand::prelude::*;
    use rstest::*;

    use super::*;

    #[rstest]
    #[case(virt_d3(0, 0, 0))]
    #[case(virt_d3(1, 2, 3))]
    #[case(virt_d3(-1, -2, -3))]
    #[case(virt_d3(i8::MAX, i8::MAX, i8::MAX))]
    #[case(virt_d3(i8::MIN, i8::MIN, i8::MIN))]
    fn virt_conversions_dont_panic(#[case] virt: VirtD3) {
        let _: Real = virt.into();
    }

    #[rstest]
    #[case(real(0, 0, 0))]
    #[case(real(1, 0, 1))]
    #[case(real(1, 0, -1))]
    #[case(real(0, -1, -1))]
    #[case(real(i8::MIN as i16, i8::MIN as i16, i8::MIN as i16))]
    #[case(real(i8::MAX as i16 - 1, i8::MAX as i16 - 1, i8::MAX as i16 - 1))]
    fn valid_real_conversions_dont_panic(#[case] real: Real) {
        let _: VirtD3 = real.try_into().unwrap();
    }

    #[rstest]
    #[case(real(1, 0, 0), RealToVirtError::NotInLattice)]
    #[case(real(i16::MAX, i16::MAX, 0), RealToVirtError::Saturation)]
    fn invalid_real_conversions_error_properly(
        #[case] real: Real,
        #[case] expected_error: RealToVirtError,
    ) {
        let res = VirtD3::try_from(real);
        assert_eq!(res, Err(expected_error));
    }

    #[rstest]
    #[case(virt_d3(0, 0, 0), real(0, 0, 0))]
    #[case(virt_d3(1, 2, 3), real(5, 4, 3))]
    #[case(virt_d3(-1, 0, 5), real(5, 4, -1))]
    fn conversion_value_spotcheck(#[case] virt: VirtD3, #[case] real: Real) {
        let convirt = real.try_into().unwrap();
        let conreal = virt.into();

        assert_eq!(virt, convirt);
        assert_eq!(real, conreal);
    }

    #[rstest]
    fn virt_real_equivalence() {
        for _ in 0..100 {
            let random_virt = virt_d3(random(), random(), random());
            println!("random_virt: {random_virt}");

            let conreal = Real::from(random_virt);
            println!("conreal: {conreal}");

            let convirt = VirtD3::try_from(conreal).unwrap();
            println!("convirt: {convirt}");

            assert_eq!(random_virt, convirt);
        }
    }
}
