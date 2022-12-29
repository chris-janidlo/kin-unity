use indextree::{Arena, NodeId};

use crate::game_state::GameState;

pub struct Searcher<T>
where
    T: GameState,
{
    arena: Arena<MctsNode<T>>,
    previous_root: Option<NodeId>,
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
            previous_root: None,
        }
    }

    pub fn search(&mut self, starting_state: T) -> T::Move {
        todo!()
    }

    fn node(&self, id: NodeId) -> &MctsNode<T> {
        self.arena.get(id).unwrap().get()
    }

    fn starting_tree(&mut self, starting_state: T) -> NodeId {
        if self.previous_root.is_none() {
            return self
                .arena
                .new_node(MctsNode::empty_from_state(starting_state));
        }

        let prev = self.previous_root.unwrap();

        let new_root = prev
            .children(&self.arena)
            .find(|id| self.node(*id).game_state == starting_state)
            .unwrap();

        new_root.detach(&mut self.arena);
        prev.remove_subtree(&mut self.arena);

        new_root
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

    fn random_node(searcher: &mut Searcher<MockGameState>) -> (MockGameState, NodeId) {
        let state: MockGameState = rand::random();
        let node_id = searcher.arena.new_node(MctsNode::empty_from_state(state));
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

        let node1 = random_node(&mut searcher);
        let node1_1 = random_node(&mut searcher);
        let node1_1_1 = random_node(&mut searcher);
        let node1_1_2 = random_node(&mut searcher);
        let node1_2 = random_node(&mut searcher);
        let node1_2_1 = random_node(&mut searcher);

        searcher.previous_root = Some(node1.1);

        node1.1.append(node1_1.1, &mut searcher.arena);
        node1.1.append(node1_2.1, &mut searcher.arena);
        node1_1.1.append(node1_1_1.1, &mut searcher.arena);
        node1_1.1.append(node1_1_2.1, &mut searcher.arena);
        node1_2.1.append(node1_2_1.1, &mut searcher.arena);

        let starting_tree = searcher.starting_tree(node1_2.0);

        let root_state = searcher.node(starting_tree).game_state;
        assert_eq!(root_state, node1_2.0);

        let mut child_states = starting_tree
            .children(&searcher.arena)
            .map(|id| searcher.node(id).game_state);
        assert_eq!(child_states.next(), Some(node1_2_1.0));
        assert_eq!(child_states.next(), None);

        let mut ancestors = starting_tree
            .ancestors(&searcher.arena)
            .map(|id| searcher.node(id).game_state);
        assert_eq!(ancestors.next(), Some(root_state));
        assert_eq!(ancestors.next(), None);

        assert!(node1.1.is_removed(&searcher.arena));
        assert!(node1_1.1.is_removed(&searcher.arena));
        assert!(node1_1_1.1.is_removed(&searcher.arena));
        assert!(node1_1_2.1.is_removed(&searcher.arena));
    }
}
