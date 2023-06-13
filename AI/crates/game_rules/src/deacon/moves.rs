use std::collections::HashSet;

use itertools::Itertools;

use super::{board::*, form::*, Piece, State};

#[derive(Debug, PartialEq, Eq, Clone, PartialOrd, Ord)]
pub struct Action {
    pub origin: Coordinate,
    pub destination: Coordinate,
    pub transformation: Form,
}

fn apply_action(board: &mut Board, action: &Action) {
    let original_piece = board.get(action.origin).as_ref().unwrap();

    let new_piece_at_origin = match original_piece.form.interaction() {
        Interaction::Swap => board.get(action.destination).clone(),
        _ => None,
    };
    let new_piece_at_destination = Some(Piece {
        form: action.transformation,
        owner: original_piece.owner,
    });

    board.set(action.origin, new_piece_at_origin);
    board.set(action.destination, new_piece_at_destination);
}

pub fn apply_move(board: &mut Board, move_: &Move) {
    for action in move_ {
        apply_action(board, action);
    }
}

pub type Move = Vec<Action>;

pub struct MoveIterator {
    moves: Vec<Move>,
}

impl MoveIterator {
    pub fn new(state: &State) -> Self {
        // current implementation: create a hashset of seen boards, and then run a for
        // loop over every possible permutation of actions, checking if the equivalent
        // move results in an unseen board. if it does, add it to a vector which is
        // returned as part of the resulting MoveIterator.

        // another possible implementation that was explored is returning the action
        // permutations as part of the MoveIterator, along with a hashset of seen boards,
        // and in MoveIterator's next function, only returning those permutations which
        // are legal and novel. the legal part was the sticking point - it's easier/
        // faster to check move legality while generating moves than after the fact, due
        // to pieces blocking each other. could come back to the idea, though

        let active_pieces = state
            .board
            .enumer_coords()
            .filter_map(|(c, o)| {
                if let Some(p) = o {
                    if p.owner == state.to_play {
                        return Some((c, p.form));
                    }
                }

                None
            })
            .collect_vec();

        let piece_count = active_pieces.len();
        let piece_permutations = active_pieces.into_iter().permutations(piece_count);

        let mut moves: Vec<Move> = Default::default();
        let mut seen_boards: HashSet<Board> = Default::default();

        // abandon hope all ye who enter here
        for permutation in piece_permutations {
            let mut possible_moves: Vec<(Move, Board)> =
                vec![(Default::default(), state.board.clone())];

            for (pnum, piece) in permutation.into_iter().enumerate() {
                let (coord, form) = piece;

                let mut new_possible_moves: Vec<(Move, Board)> = Default::default();

                // TODO: can we combine this into one iproduct loop, for less nesting?
                // would need to figure out how to only break out of the range loop in
                // the dest.is_err() and targeted_piece.is_none() checks. take_while?
                for (move_, board) in &possible_moves {
                    for direction in form.directions() {
                        'range: for i in form.range() {
                            let dest = coord.checked_add(direction * (i as i8));

                            if dest.is_err() {
                                break 'range;
                            }
                            let destination = dest.unwrap();

                            let targeted_piece = board.get(destination);

                            let valid_action = targeted_piece.is_none()
                                || targeted_piece.as_ref().map_or(false, |t| {
                                    form.interaction() != &Interaction::None
                                        && t.owner != state.to_play
                                });

                            if valid_action {
                                for transformation in form.transformations() {
                                    let action = Action {
                                        origin: coord,
                                        destination,
                                        transformation,
                                    };

                                    let mut new_board = board.clone();
                                    apply_action(&mut new_board, &action);

                                    let mut new_move = move_.clone();
                                    new_move.push(action);

                                    new_possible_moves.push((new_move, new_board));
                                }
                            }

                            if targeted_piece.is_some() {
                                break 'range;
                            }
                        }
                    }
                }

                possible_moves.extend(
                    new_possible_moves
                        .into_iter()
                        .filter(|m| m.0.len() == pnum + 1),
                );
            }

            // TODO: can we move this inward at all to save on iterations?
            for (move_, board) in possible_moves {
                if move_.len() != piece_count {
                    // skip partial moves that were unable to fully expand
                    continue;
                }

                if !seen_boards.contains(&board) {
                    moves.push(move_);
                    seen_boards.insert(board);
                }
            }
        }

        MoveIterator { moves }
    }
}

impl Iterator for MoveIterator {
    type Item = Move;

    fn next(&mut self) -> Option<Self::Item> {
        self.moves.pop()
    }
}

#[cfg(test)]
mod tests {
    use rstest::*;

    use super::*;

    #[rstest(
        start_board, action, expected_result,
        case::basic_scientist_movement(
            board![
                [S, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
            ],
            Action {
                origin: coord_unwrapped(0, 0),
                destination: coord_unwrapped(1, 1),
                transformation: Form::Engineer
            },
            board![
                [_, _, _, _, _];
                [_, E, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
            ],
        ),
        case::basic_scientist_movement_with_enemy_on_board(
            board![
                [S, _, r, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
            ],
            Action {
                origin: coord_unwrapped(0, 0),
                destination: coord_unwrapped(1, 1),
                transformation: Form::Engineer
            },
            board![
                [_, _, r, _, _];
                [_, E, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
            ],
        ),
        case::capture(
            board![
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, E, _, _];
                [_, _, _, _, _];
                [c, _, _, _, _];
            ],
            Action {
                origin: coord_unwrapped(0, 4),
                destination: coord_unwrapped(2, 2),
                transformation: Form::Scientist
            },
            board![
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, s, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
            ],
        ),
        case::swap(
            board![
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, c, _, _];
                [_, _, _, _, _];
                [_, _, P, _, _];
            ],
            Action {
                origin: coord_unwrapped(2, 4),
                destination: coord_unwrapped(2, 2),
                transformation: Form::Engineer
            },
            board![
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, E, _, _];
                [_, _, _, _, _];
                [_, _, c, _, _];
            ],
        ),
    )]

    fn test_apply_action(mut start_board: Board, action: Action, expected_result: Board) {
        apply_action(&mut start_board, &action);
        assert_eq!(start_board, expected_result);
    }

    #[rstest(
        start_board, move_, expected_result,
        case::basic_double_movement(
            board![
                [_, S, _, _, _];
                [S, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
            ],
            vec![
                Action {
                    origin: coord_unwrapped(0, 1),
                    destination: coord_unwrapped(1, 1),
                    transformation: Form::Engineer
                },
                Action {
                    origin: coord_unwrapped(1, 0),
                    destination: coord_unwrapped(0, 0),
                    transformation: Form::Priest
                },
            ],
            board![
                [P, _, _, _, _];
                [_, E, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
            ],
        ),
        case::double_movement_with_enemies_on_board(
            board![
                [_, _, _, _, _];
                [_, _, _, E, _];
                [_, _, _, _, _];
                [_, I, _, _, c];
                [_, _, _, c, _];
            ],
            vec![
                Action {
                    origin: coord_unwrapped(3, 1),
                    destination: coord_unwrapped(2, 0),
                    transformation: Form::Pilot
                },
                Action {
                    origin: coord_unwrapped(1, 3),
                    destination: coord_unwrapped(0, 2),
                    transformation: Form::Captain
                },
            ],
            board![
                [_, _, I, _, _];
                [_, _, _, _, _];
                [C, _, _, _, _];
                [_, _, _, _, c];
                [_, _, _, c, _];
            ],
        ),
        case::double_capture(
            board![
                [_, _, _, _, _];
                [_, _, _, r, _];
                [_, _, _, _, _];
                [_, i, _, S, _];
                [_, _, C, _, _];
            ],
            vec![
                Action {
                    origin: coord_unwrapped(3, 1),
                    destination: coord_unwrapped(3, 3),
                    transformation: Form::Priest
                },
                Action {
                    origin: coord_unwrapped(1, 3),
                    destination: coord_unwrapped(2, 4),
                    transformation: Form::Priest
                },
            ],
            board![
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, _, _];
                [_, _, _, p, _];
                [_, _, p, _, _];
            ],
        ),
        case::swap_into_capture(
            board![
                [_, _, _, _, _];
                [_, e, _, r, _];
                [_, _, _, _, _];
                [_, _, _, S, _];
                [_, _, _, _, _];
            ],
            vec![
                Action {
                    origin: coord_unwrapped(1, 1),
                    destination: coord_unwrapped(3, 3),
                    transformation: Form::Pilot
                },
                Action {
                    origin: coord_unwrapped(3, 1),
                    destination: coord_unwrapped(1, 1),
                    transformation: Form::Engineer
                },
            ],
            board![
                [_, _, _, _, _];
                [_, e, _, _, _];
                [_, _, _, _, _];
                [_, _, _, i, _];
                [_, _, _, _, _];
            ],
        ),
    )]
    fn test_apply_move(mut start_board: Board, move_: Move, expected_result: Board) {
        apply_move(&mut start_board, &move_);
        assert_eq!(start_board, expected_result);
    }
}

// split this into a separate file because the test cases are hundreds of lines long
#[cfg(test)]
mod move_iterator_tests;
