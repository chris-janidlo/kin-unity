use std::ops::{Index, IndexMut};

use glam::*;
use ndarray::*;

use super::{direction::Axis, SpicePlayer};

pub const GRID_CONSTANT_F: f32 = std::f32::consts::TAU;
pub const GRID_CONSTANT_I: i32 = 7; // GRID_CONSTANT_F.ceil(), hardcoded bc ceil() isn't const

#[derive(PartialEq, Clone)]
pub struct Grid {
    // see dynalist for notes on how to make this more space-efficient
    values: Array3<GridSpace>,
}

#[derive(Debug, PartialEq, Clone, Copy)]
pub enum GridSpace {
    Illegal,
    Empty,
    Blocked,
    Line(SpicePlayer, Axis),
    Endpoint {
        owner: SpicePlayer,
        connected_lines: u8,
    },
}

fn to_nd(vec: IVec3) -> [usize; 3] {
    let x = (vec.x + GRID_CONSTANT_I) as usize;
    let y = (vec.y + GRID_CONSTANT_I) as usize;
    let z = (vec.z + GRID_CONSTANT_I) as usize;

    [x, y, z]
}

impl Default for Grid {
    fn default() -> Self {
        let grid_width = Self::axis_length() as usize;

        let mut result = Self {
            values: Array3::from_elem((grid_width, grid_width, grid_width), GridSpace::Illegal),
        };

        for i in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
            for j in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                for k in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                    if (i + j + k) % 2 != 0 {
                        continue;
                    }

                    let vec: Vec3 = vec3(i as f32, j as f32, k as f32);
                    if vec.length_squared() >= GRID_CONSTANT_F * GRID_CONSTANT_F {
                        continue;
                    }

                    result[vec.as_ivec3()] = GridSpace::Empty;
                }
            }
        }

        result
    }
}

impl Index<IVec3> for Grid {
    type Output = GridSpace;

    fn index(&self, idx: IVec3) -> &Self::Output {
        &self.values[to_nd(idx)]
    }
}

impl IndexMut<IVec3> for Grid {
    fn index_mut(&mut self, idx: IVec3) -> &mut Self::Output {
        &mut self.values[to_nd(idx)]
    }
}

impl Grid {
    pub fn get(&self, idx: IVec3) -> Option<GridSpace> {
        self.values.get(to_nd(idx)).copied()
    }

    pub fn get_legal(&self, idx: IVec3) -> Option<GridSpace> {
        self.get(idx).and_then(|s| match s {
            GridSpace::Illegal => None,
            _ => Some(s),
        })
    }

    pub fn axis_length() -> i32 {
        // `+ 1` to include 0
        GRID_CONSTANT_I * 2 + 1
    }

    pub fn slice_mut(
        &mut self,
        info: impl SliceArg<Ix3, OutDim = Dim<[usize; 3]>>,
    ) -> ArrayViewMut3<GridSpace> {
        self.values.slice_mut(info)
    }
}

#[cfg(test)]
mod tests {
    use std::time::Duration;

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
    fn default_has_correct_shape(empty_grid: Grid) {
        let mut axis_lengths = empty_grid.values.axes().map(|a| a.len as i32);

        assert_eq!(axis_lengths.next(), Some(Grid::axis_length()));
        assert_eq!(axis_lengths.next(), Some(Grid::axis_length()));
        assert_eq!(axis_lengths.next(), Some(Grid::axis_length()));
        assert_eq!(axis_lengths.next(), None);
    }

    #[rstest]
    fn index_offsets_correctly(empty_grid: Grid) {
        // the ndarray that Grid uses starts at (0, 0, 0), while we want a public API
        // that starts at (GRID_CONSTANT_I...)
        let offset = Grid::axis_length() / 2;

        let idx_btm = IVec3::splat(-offset);
        let idx_mid = IVec3::ZERO;
        let idx_top = IVec3::splat(offset);

        let bottom_checked = empty_grid.get(idx_btm);
        let middle_checked = empty_grid.get(idx_mid);
        let top_checked = empty_grid.get(idx_top);

        assert_ne!(bottom_checked, None, "grid has no value at {idx_btm}");
        assert_ne!(middle_checked, None, "grid has no value at {idx_mid}");
        assert_ne!(top_checked, None, "grid has no value at {idx_top}");

        let bottom_unchecked = empty_grid[idx_btm];
        let middle_unchecked = empty_grid[idx_mid];
        let top_unchecked = empty_grid[idx_top];

        assert_eq!(bottom_unchecked, bottom_checked.unwrap());
        assert_eq!(middle_unchecked, middle_checked.unwrap());
        assert_eq!(top_unchecked, top_checked.unwrap());
    }

    #[rstest]
    fn default_generates_correct_grid(empty_grid: Grid) {
        for i in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
            for j in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                for k in -GRID_CONSTANT_I..=GRID_CONSTANT_I {
                    let v = ivec3(i, j, k);

                    if let GridSpace::Illegal = empty_grid[v] {
                        let out_of_bounds = v.as_vec3().length() >= GRID_CONSTANT_F;
                        let not_in_lattice = (i + j + k) % 2 != 0;

                        assert!(
                            out_of_bounds || not_in_lattice,
                            "out of bounds: {out_of_bounds}, not in lattice: {not_in_lattice}"
                        );
                    }
                }
            }
        }
    }
}
