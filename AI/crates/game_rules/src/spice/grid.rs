use ndarray::Array3;

use super::{coord::*, direction::*, players::*};

pub const GRID_CONSTANT_F: f32 = 5.2;
pub const GRID_CONSTANT_I: i8 = 5; // GRID_CONSTANT_F.floor(), hardcoded bc floor() isn't const

#[derive(Debug, PartialEq, Clone)]
pub struct Grid {
    // spaces are indexed by VirtD3s, offset by a constant factor
    packed_spaces: Array3<Option<GridSpace>>,
}

#[derive(Debug, PartialEq, Clone)]
pub enum GridSpace {
    Empty,
    Blocked,
    LineSegment {
        axis: Axis,
        hardened: bool,
    },
    Endpoint {
        owner: SpicePlayer,
        connected_lines: u8,
    },
}

impl Default for Grid {
    fn default() -> Self {
        let grid_width = Self::axis_length();

        let mut result = Self {
            packed_spaces: Array3::from_elem((grid_width, grid_width, grid_width), None),
        };

        for i in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
            for j in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                for k in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                    let virt = virt_d3(i, j, k);

                    if Self::in_sphere(virt) {
                        result.packed_spaces[Self::indexify(virt)] = Some(GridSpace::Empty);
                    }
                }
            }
        }

        result
    }
}

impl Grid {
    pub fn enumerate_vc(&self) -> impl Iterator<Item = (VirtD3, &GridSpace)> {
        self.packed_spaces
            .indexed_iter()
            .filter_map(|(c, s)| s.as_ref().map(|space| (Self::unindexify(c), space)))
    }

    /// Attempt to retrieve a space in the grid, using a [Real] coordinate.
    pub fn get_rc(&self, index: Real) -> Option<&GridSpace> {
        match index.try_into() {
            Ok(v) => self.get_vc(v),
            Err(_) => None,
        }
    }

    /// Attempt to retrieve a space in the grid, using a [VirtD3] coordinate.
    pub fn get_vc(&self, index: VirtD3) -> Option<&GridSpace> {
        let idx = Self::indexify(index);
        self.packed_spaces.get(idx).and_then(|s| s.as_ref())
    }

    pub fn get_mut_vc(&mut self, index: VirtD3) -> Option<&mut GridSpace> {
        let idx = Self::indexify(index);
        self.packed_spaces.get_mut(idx).and_then(|s| s.as_mut())
    }

    /// Attempt to set a space in the grid, using a [Real] coordinate.
    pub fn set_rc(&mut self, index: Real, value: GridSpace) -> Result<(), String> {
        let virt: Result<VirtD3, _> = index.try_into();

        match virt {
            Err(e) => Err(format!("{index:?} cannot be converted to virt ({e:?})")),
            Ok(v) => self
                .set_vc(v, value)
                .map_err(|e| format!("unable to set {index}: {e}")),
        }
    }

    /// Attempt to set a space in the grid, using a [VirtD3] coordinate.
    pub fn set_vc(&mut self, index: VirtD3, value: GridSpace) -> Result<(), String> {
        if !Self::in_sphere(index) {
            return Err(format!("{index:?} is out of sphere bounds"));
        }

        let idx = Self::indexify(index);

        let (i, j, k) = idx;
        if i > Self::axis_length() || j > Self::axis_length() || k > Self::axis_length() {
            return Err(format!("{idx:?} is out of array bounds"));
        }

        self.packed_spaces[idx] = Some(value);

        Ok(())
    }

    /// Set a space in the grid, using a [Real] coordinate.
    ///
    /// # Panics
    ///
    /// Panics if the coordinate isn't in D3, or is out of the bounds of the array.
    pub fn set_rc_unchecked(&mut self, index: Real, value: GridSpace) {
        self.set_vc_unchecked(index.try_into().unwrap(), value);
    }

    /// Set a space in the grid, using a [VirtD3] coordinate.
    ///
    /// # Panics
    ///
    /// Panics if the coordinate is out of the bounds of the array.
    pub fn set_vc_unchecked(&mut self, index: VirtD3, value: GridSpace) {
        self.packed_spaces[Self::indexify(index)] = Some(value);
    }

    pub fn is_valid_and_empty_vc(&self, index: VirtD3) -> bool {
        matches!(
            self.packed_spaces.get(Self::indexify(index)),
            Some(Some(GridSpace::Empty))
        )
    }

    #[inline]
    fn axis_length() -> usize {
        // each axis is two spans of GRID_CONSTANT_I length, plus 1 space for the origin
        (GRID_CONSTANT_I * 2 + 1) as usize
    }

    #[inline]
    fn indexify(virt: VirtD3) -> (usize, usize, usize) {
        let VirtD3 { i, j, k } = virt;

        (
            (i + GRID_CONSTANT_I) as usize,
            (j + GRID_CONSTANT_I) as usize,
            (k + GRID_CONSTANT_I) as usize,
        )
    }

    #[inline]
    fn unindexify(idx: (usize, usize, usize)) -> VirtD3 {
        let (t, u, v) = idx;

        virt_d3(
            t as i8 - GRID_CONSTANT_I,
            u as i8 - GRID_CONSTANT_I,
            v as i8 - GRID_CONSTANT_I,
        )
    }

    #[inline]
    fn in_sphere(virt: VirtD3) -> bool {
        virt.length_squared() < GRID_CONSTANT_F * GRID_CONSTANT_F
    }
}

#[cfg(test)]
mod tests {
    use std::time::Duration;

    use pretty_assertions::assert_eq;
    use rand::prelude::*;
    use rstest::*;

    use super::*;

    #[fixture]
    fn empty_grid() -> Grid {
        Grid::default()
    }

    #[rstest]
    #[timeout(Duration::from_millis(10))]
    fn default_works() {
        Grid::default();
    }

    #[rstest]
    fn index_spotcheck(empty_grid: Grid) {
        // set offset to the largest possible integer such that a basis Z^3 vector
        // multiplied by offset is both 1) in the FCC lattice and 2) within a radius of
        // GRID_CONSTANT
        let mut offset = GRID_CONSTANT_F.floor() as i16;
        if offset % 2 != 0 {
            // ensure offset is even, to satisfy FCC constraint
            // and round down, to satisfy radius constraint
            offset -= 1;
        }

        let indices = [
            real(0, 0, 0),
            real(offset, 0, 0),
            real(-offset, 0, 0),
            real(0, offset, 0),
            real(0, -offset, 0),
            real(0, 0, offset),
            real(0, 0, -offset),
        ];

        for index in indices {
            let space = empty_grid.get_rc(index);
            assert_ne!(space, None, "{index} should be a valid space");
        }
    }

    #[rstest]
    fn default_generates_correct_grid(empty_grid: Grid) {
        for x in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
            for y in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                for z in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                    let c = real(x.into(), y.into(), z.into());
                    let s = empty_grid.get_rc(c);

                    match VirtD3::try_from(c) {
                        Ok(v) => match s {
                            Some(_) => assert!(
                                v.length() < GRID_CONSTANT_F,
                                "valid spaces must be inside the sphere"
                            ),
                            None => assert!(
                                v.length() >= GRID_CONSTANT_F,
                                "invalid spaces that are in the lattice must be outside the sphere"
                            ),
                        },
                        Err(_) => assert!(
                            s.is_none(),
                            "spaces not in the lattice cannot be in the grid"
                        ),
                    }
                }
            }
        }
    }

    #[rstest]
    #[case(real(0, 0, 0), true)]
    #[case(real(1, 0, 1), true)]
    #[case(real(1, -2, 1), true)]
    #[case(real(0, 0, 4), true)]
    #[case(real(0, 1, 0), false)]
    #[case(real(4, 4, -4), false)]
    #[case(real(i8::MIN.into(), i8::MIN.into(), i8::MIN.into()), false)]
    #[case(real(i16::MIN, i16::MIN, i16::MIN), false)]
    fn spotcheck_set_rc(mut empty_grid: Grid, #[case] coord: Real, #[case] ok: bool) {
        assert_eq!(empty_grid.set_rc(coord, GridSpace::Empty).is_ok(), ok);
    }

    #[rstest]
    #[case(virt_d3(0, 0, 0), true)]
    #[case(virt_d3(1, 2, 3), true)]
    #[case(virt_d3(1, 1, 1), true)]
    #[case(virt_d3(-1, -2, -3), true)]
    #[case(virt_d3(-1, -1, -1), true)]
    #[case(virt_d3(6, 5, 5), false)]
    #[case(virt_d3(i8::MAX, i8::MAX, i8::MAX), false)]
    #[case(virt_d3(i8::MIN, i8::MIN, i8::MIN), false)]
    fn spotcheck_set_vc(mut empty_grid: Grid, #[case] coord: VirtD3, #[case] ok: bool) {
        assert_eq!(empty_grid.set_vc(coord, GridSpace::Empty).is_ok(), ok);
    }

    #[rstest]
    #[case(real(0, 0, 0))]
    #[case(real(1, 0, 1))]
    #[case(real(1, -2, 1))]
    #[case(real(0, 0, 4))]
    #[should_panic]
    #[case(real(0, 1, 0))]
    #[should_panic]
    #[case(real(4, 4, -4))]
    #[should_panic]
    #[case(real(i8::MIN.into(), i8::MIN.into(), i8::MIN.into()))]
    #[should_panic]
    #[case(real(i16::MIN, i16::MIN, i16::MIN))]
    fn spotcheck_set_rc_unchecked(mut empty_grid: Grid, #[case] coord: Real) {
        empty_grid.set_rc_unchecked(coord, GridSpace::Empty);
    }

    #[rstest]
    #[case(virt_d3(0, 0, 0))]
    #[case(virt_d3(1, 2, 3))]
    #[case(virt_d3(1, 1, 1))]
    #[case(virt_d3(-1, -2, -3))]
    #[case(virt_d3(-1, -1, -1))]
    #[should_panic]
    #[case(virt_d3(6, 5, 5))]
    #[should_panic]
    #[case(virt_d3(i8::MAX, i8::MAX, i8::MAX))]
    #[should_panic]
    #[case(virt_d3(i8::MIN, i8::MIN, i8::MIN))]
    fn spotcheck_set_vc_unchecked(mut empty_grid: Grid, #[case] coord: VirtD3) {
        empty_grid.set_vc_unchecked(coord, GridSpace::Empty);
    }

    #[rstest]
    fn enumerate_vc_doesnt_panic(empty_grid: Grid) {
        for e in empty_grid.enumerate_vc() {
            println!("{e:?}");
        }
    }

    #[rstest]
    fn enumerate_vc_visits_whole_grid(empty_grid: Grid) {
        let mut expected_path: Vec<VirtD3> = Default::default();
        for i in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
            for j in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                for k in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                    let c = virt_d3(i, j, k);
                    if c.length() < GRID_CONSTANT_F {
                        expected_path.push(c);
                    }
                }
            }
        }
        expected_path.sort_unstable_by_key(|c| (c.i, c.j, c.k));

        let mut actual_path: Vec<VirtD3> = empty_grid.enumerate_vc().map(|(c, _)| c).collect();
        actual_path.sort_unstable_by_key(|c| (c.i, c.j, c.k));

        assert_eq!(actual_path, expected_path);
    }

    #[rstest]
    fn indexify_unindexify_equivalence() {
        for _ in 0..100 {
            let virt = virt_d3(
                thread_rng().gen_range(i8::MIN..=i8::MAX - GRID_CONSTANT_I),
                thread_rng().gen_range(i8::MIN..=i8::MAX - GRID_CONSTANT_I),
                thread_rng().gen_range(i8::MIN..=i8::MAX - GRID_CONSTANT_I),
            );
            let index = Grid::indexify(virt);
            let unindex = Grid::unindexify(index);

            assert_eq!(
                virt, unindex,
                "{index:?} was improperly generated or converted"
            );
        }
    }

    #[rstest]
    #[case(virt_d3(0, 0, 0))]
    #[case(virt_d3(1, 2, 3))]
    #[case(virt_d3(3, 3, 3))]
    #[case(virt_d3(-3, -3, -3))]
    fn is_valid_and_empty_vc_is_true_for_empty_spaces_in_the_grid(
        empty_grid: Grid,
        #[case] coord: VirtD3,
    ) {
        assert!(empty_grid.is_valid_and_empty_vc(coord));
    }

    #[rstest]
    #[case(virt_d3(5, 5, 5))]
    #[case(virt_d3(i8::MAX, i8::MAX, i8::MAX))]
    #[case(virt_d3(-4, -4, 4))]
    #[case(virt_d3(i8::MIN, i8::MIN, i8::MIN))]
    fn is_valid_and_empty_vc_is_false_outside_the_grid(empty_grid: Grid, #[case] coord: VirtD3) {
        assert!(!empty_grid.is_valid_and_empty_vc(coord));
    }

    #[rstest]
    fn is_valid_and_empty_vc_is_false_for_non_empty(mut empty_grid: Grid) {
        empty_grid.set_vc_unchecked(virt_d3(0, 0, 0), GridSpace::Blocked);
        assert!(!empty_grid.is_valid_and_empty_vc(virt_d3(0, 0, 0)));
    }
}
