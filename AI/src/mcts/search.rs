use indextree::{Arena, NodeId};

use crate::game_state::GameState;

pub struct Searcher<T>
where
    T: GameState,
{
    arena: Arena<MctsNode<T>>,
    previous_choice: Option<NodeId>,
}

#[derive(PartialEq, Debug)]
struct MctsNode<T>
where
    T: GameState,
{
    game_state: T,
    score: f32,
    visits: i32,
}

impl<T> MctsNode<T>
where
    T: GameState,
{
    pub fn empty_from_state(game_state: T) -> Self {
        MctsNode {
            game_state,
            score: 0.,
            visits: 0,
        }
    }
}

impl<T> Searcher<T>
where
    T: GameState,
{
    pub fn new() -> Self {
        Searcher {
            arena: Arena::new(),
            previous_choice: None,
        }
    }

    pub fn search(&mut self, _starting_state: T) -> T::Move {
        todo!()
    }

    fn node(&self, id: NodeId) -> &MctsNode<T> {
        self.arena.get(id).unwrap().get()
    }

    fn starting_tree(&mut self, starting_state: T) -> NodeId {
        if self.previous_choice.is_none() {
            return self
                .arena
                .new_node(MctsNode::empty_from_state(starting_state));
        }

        let old_root = self.previous_choice.unwrap();

        let child_move = old_root
            .children(&self.arena)
            .find(|id| self.node(*id).game_state == starting_state);

        if let Some(new_root) = child_move {
            new_root.detach(&mut self.arena);
            old_root.remove_subtree(&mut self.arena);

            new_root
        } else {
            self.arena.clear();

            self.arena
                .new_node(MctsNode::empty_from_state(starting_state))
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    type MockGameState = i32;

    impl GameState for MockGameState {
        type Move = i32;
        type MoveIterator = std::vec::IntoIter<Self::Move>;

        fn initial_state() -> Self {
            0
        }

        fn available_moves(&self) -> Self::MoveIterator {
            vec![1, 2, 3].into_iter()
        }

        fn apply_move(&self, move_: Self::Move) -> Self {
            self + move_
        }
    }

    fn random_node(
        searcher: &mut Searcher<MockGameState>,
        parent: Option<NodeId>,
    ) -> (MockGameState, NodeId) {
        let state: MockGameState = rand::random();
        let node_id = searcher.arena.new_node(MctsNode::empty_from_state(state));

        if let Some(parent_id) = parent {
            parent_id.append(node_id, &mut searcher.arena);
        }

        (state, node_id)
    }

    #[test]
    fn starting_tree_creates_new_tree() {
        let mut searcher: Searcher<MockGameState> = Searcher::new();

        let state: MockGameState = rand::random();
        let starting_tree = searcher.starting_tree(state);

        assert_eq!(searcher.node(starting_tree).game_state, state);
    }

    #[test]
    fn starting_tree_finds_old_tree_and_detaches() {
        let mut searcher: Searcher<MockGameState> = Searcher::new();

        let node_1 = random_node(&mut searcher, None);
        let node_1_1 = random_node(&mut searcher, Some(node_1.1));
        let node_1_1_1 = random_node(&mut searcher, Some(node_1_1.1));
        let node_1_1_2 = random_node(&mut searcher, Some(node_1_1.1));
        let node_1_2 = random_node(&mut searcher, Some(node_1.1));
        let node_1_2_1 = random_node(&mut searcher, Some(node_1_2.1));

        searcher.previous_choice = Some(node_1.1);

        let starting_tree = searcher.starting_tree(node_1_2.0);

        let root_state = searcher.node(starting_tree).game_state;
        assert_eq!(root_state, node_1_2.0);

        let mut child_states = starting_tree
            .children(&searcher.arena)
            .map(|id| searcher.node(id).game_state);
        assert_eq!(child_states.next(), Some(node_1_2_1.0));
        assert_eq!(child_states.next(), None);

        let mut ancestors = starting_tree
            .ancestors(&searcher.arena)
            .map(|id| searcher.node(id).game_state);
        assert_eq!(ancestors.next(), Some(root_state));
        assert_eq!(ancestors.next(), None);

        assert!(node_1.1.is_removed(&searcher.arena));
        assert!(node_1_1.1.is_removed(&searcher.arena));
        assert!(node_1_1_1.1.is_removed(&searcher.arena));
        assert!(node_1_1_2.1.is_removed(&searcher.arena));
    }

    #[test]
    fn starting_tree_creates_new_if_children_dont_match() {
        let mut searcher: Searcher<MockGameState> = Searcher::new();

        let node_1 = random_node(&mut searcher, None);
        let node_1_1 = random_node(&mut searcher, Some(node_1.1));
        let node_1_2 = random_node(&mut searcher, Some(node_1.1));

        searcher.previous_choice = Some(node_1.1);

        let mut other_data: MockGameState = rand::random();
        while other_data == node_1_1.0 || other_data == node_1_2.0 {
            other_data = rand::random();
        }

        let starting_tree = searcher.starting_tree(other_data);

        let root_state = searcher.node(starting_tree).game_state;
        assert_eq!(root_state, other_data);

        let mut child_states = starting_tree
            .children(&searcher.arena)
            .map(|id| searcher.node(id).game_state);
        assert_eq!(child_states.next(), None);

        let mut ancestors = starting_tree
            .ancestors(&searcher.arena)
            .map(|id| searcher.node(id).game_state);
        assert_eq!(ancestors.next(), Some(other_data));
        assert_eq!(ancestors.next(), None);
    }
}
