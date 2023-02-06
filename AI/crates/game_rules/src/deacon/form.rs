use std::{collections::HashMap, ops::RangeInclusive, vec};

use lazy_static::lazy_static;

use super::board::*;

#[derive(PartialEq, Eq)]
pub enum Interaction {
    None,
    Capture,
    Swap,
}

#[derive(Debug, Hash, PartialEq, Eq, Clone, Copy)]
pub enum Directions {
    Cardinal,
    Diagonal,
    All,
}

#[derive(Debug, Hash, PartialEq, Eq, Clone, Copy, PartialOrd, Ord)]
pub enum Form {
    Captain,
    Engineer,
    Pilot,
    Priest,
    Robot,
    Scientist,
}

struct MovementRules {
    range: u8,
    directions: Directions,
    interaction: Interaction,
    transformations: [Option<Form>; 3],
}

lazy_static! {
    static ref FORM_DATA: HashMap<Form, &'static MovementRules> = HashMap::from([
        (
            Form::Captain,
            &MovementRules {
                range: 5,
                directions: Directions::All,
                interaction: Interaction::Capture,
                transformations: [Some(Form::Scientist), None, None]
            }
        ),
        (
            Form::Engineer,
            &MovementRules {
                range: 2,
                directions: Directions::Diagonal,
                interaction: Interaction::Swap,
                transformations: [Some(Form::Pilot), Some(Form::Priest), None]
            }
        ),
        (
            Form::Pilot,
            &MovementRules {
                range: 2,
                directions: Directions::Diagonal,
                interaction: Interaction::Capture,
                transformations: [
                    Some(Form::Engineer),
                    Some(Form::Priest),
                    Some(Form::Captain)
                ]
            }
        ),
        (
            Form::Priest,
            &MovementRules {
                range: 3,
                directions: Directions::Cardinal,
                interaction: Interaction::Swap,
                transformations: [Some(Form::Robot), Some(Form::Engineer), None]
            }
        ),
        (
            Form::Robot,
            &MovementRules {
                range: 2,
                directions: Directions::Cardinal,
                interaction: Interaction::Capture,
                transformations: [
                    Some(Form::Engineer),
                    Some(Form::Priest),
                    Some(Form::Captain)
                ]
            }
        ),
        (
            Form::Scientist,
            &MovementRules {
                range: 1,
                directions: Directions::All,
                interaction: Interaction::None,
                transformations: [Some(Form::Engineer), Some(Form::Priest), None]
            }
        ),
    ]);
    static ref DIRECTION_DATA: HashMap<Directions, Vec<RelCoord>> = HashMap::from([
        (
            Directions::Cardinal,
            vec![rcoord(1, 0), rcoord(0, 1), rcoord(-1, 0), rcoord(0, -1),]
        ),
        (
            Directions::Diagonal,
            vec![rcoord(1, 1), rcoord(-1, 1), rcoord(1, -1), rcoord(-1, -1)]
        ),
        (
            Directions::All,
            vec![
                rcoord(1, 0),
                rcoord(0, 1),
                rcoord(-1, 0),
                rcoord(0, -1),
                rcoord(1, 1),
                rcoord(-1, 1),
                rcoord(1, -1),
                rcoord(-1, -1),
            ]
        ),
    ]);
}

impl Form {
    pub fn range(&self) -> RangeInclusive<u8> {
        let max_range = FORM_DATA[self].range;
        1..=max_range
    }

    pub fn directions(&self) -> Vec<RelCoord> {
        let dir = FORM_DATA[self].directions;
        DIRECTION_DATA[&dir].clone()
    }

    pub fn interaction(&self) -> &Interaction {
        &FORM_DATA[self].interaction
    }

    pub fn transformations(&self) -> Vec<Form> {
        FORM_DATA[self]
            .transformations
            .iter()
            .filter_map(|x| x.as_ref())
            .cloned()
            .collect()
    }
}
