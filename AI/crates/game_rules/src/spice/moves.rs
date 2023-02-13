use super::{coord::*, direction::*};

pub struct SpiceMove {
    source: VirtD3,
    direction: Direction,
}

pub struct SpiceMoveIterator {}

impl Iterator for SpiceMoveIterator {
    type Item = SpiceMove;

    fn next(&mut self) -> Option<Self::Item> {
        todo!()
    }
}
