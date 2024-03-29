//! Implementation of the MCTS algorithm as described by Browne et al 2012

use indextree::{Arena, NodeId};

use super::SearchParameters;
use crate::game_state::GameState;

pub struct Searcher<T>
where
    T: GameState,
{
    arena: Arena<MctsNode<T>>,
    previous_choice: Option<NodeId>,
    parameters: SearchParameters,
}

#[derive(PartialEq, Debug)]
struct MctsNode<T>
where
    T: GameState,
{
    game_state: T,
    move_: T::Move,
    score: f32,
    visits: i32,
    unexpanded_moves: T::MoveIterator,
}

impl<T> Searcher<T>
where
    T: GameState,
{
    pub fn new(parameters: SearchParameters) -> Self {
        Searcher {
            arena: Arena::new(),
            previous_choice: None,
            parameters,
        }
    }

    pub fn search(&mut self, starting_state: T) -> T::Move {
        let player = starting_state.next_to_play();
        let root = self.starting_tree(starting_state);

        for _ in 0..self.parameters.search_iterations {
            let leaf = self.tree_policy(root);
            let leaf_state = &self.node(leaf).game_state;
            let score = Self::rollout(leaf_state, player);
            self.backup_negamax(leaf, score);
        }

        let max_child = self.best_child(root, 0.);
        self.previous_choice = Some(max_child);

        self.node(max_child).move_.clone()
    }

    fn node(&self, id: NodeId) -> &MctsNode<T> {
        self.arena.get(id).unwrap().get()
    }

    fn node_mut(&mut self, id: NodeId) -> &mut MctsNode<T> {
        self.arena.get_mut(id).unwrap().get_mut()
    }

    fn starting_tree(&mut self, starting_state: T) -> NodeId {
        if self.previous_choice.is_none() {
            return self
                .arena
                .new_node(MctsNode::new(starting_state, Default::default()));
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
                .new_node(MctsNode::new(starting_state, Default::default()))
        }
    }

    fn rollout(initial_state: &T, for_player: T::Player) -> f32 {
        // wanted to do this iteratively, but was fighting the borrow checker. hopefully
        // we'll see some tail call optimization
        if let Some(val) = initial_state.terminal_value(for_player) {
            val
        } else {
            let (_, state) = initial_state
                .default_policy(&mut initial_state.available_moves())
                .expect("there should be moves to explore if the node is not terminal");

            Self::rollout(&state, for_player)
        }
    }

    fn tree_policy(&mut self, mut node_id: NodeId) -> NodeId {
        loop {
            let state = &self.node(node_id).game_state;
            if state.terminal_value(state.next_to_play()).is_some() {
                return node_id;
            }

            if let Some(leaf) = self.expand(node_id) {
                return leaf;
            }

            node_id = self.best_child(node_id, self.parameters.exploration_factor);
        }
    }

    fn expand(&mut self, node_id: NodeId) -> Option<NodeId> {
        let node = self.node_mut(node_id);

        node.game_state
            .default_policy(&mut node.unexpanded_moves)
            .map(|(move_, game_state)| {
                let child = self.arena.new_node(MctsNode::new(game_state, move_));
                node_id.append(child, &mut self.arena);

                child
            })
    }

    /// exploration_factor is also known as c (Browne p. 9)
    fn best_child(&self, parent: NodeId, exploration_factor: f32) -> NodeId {
        let ucb1 = |id: &NodeId| {
            let parent = self.node(parent);
            let child = self.node(*id);

            let exploitation_term = child.score / child.visits_f();
            let exploration_term = (2. * parent.visits_f().ln() / child.visits_f()).sqrt();

            exploitation_term + exploration_factor * exploration_term
        };

        parent
            .children(&self.arena)
            .max_by(|a, b| {
                let a_val = ucb1(a);
                let b_val = ucb1(b);

                // `max_by` is slightly biased toward elements at the end. and while
                // it's probably very unlikely that unwrapping to Equal by defualt has
                // a major effect, any distortions would cause actions at the end of the
                // iterator to be favored. if this becomes a problem, might want to
                // unwrap to a random comparison by default
                a_val
                    .partial_cmp(&b_val)
                    .unwrap_or(std::cmp::Ordering::Equal)
            })
            .unwrap()
    }

    fn backup_negamax(&mut self, node_id: NodeId, mut score: f32) {
        // would prefer a for loop over `node_id.ancestors`, but that seems to lead to a
        // borrow conflict, due to needing to borrow the arena immutably in the iterator
        // call and mutably in the call to modify the node. would like to try again and
        // see if that borrow conflict can be worked around

        let mut next = self.arena.get_mut(node_id);

        while let Some(node) = next {
            let data = node.get_mut();
            data.score += score;
            data.visits += 1;

            score = -score;
            next = node.parent().and_then(|id| self.arena.get_mut(id));
        }
    }
}

impl<T> MctsNode<T>
where
    T: GameState,
{
    fn new(game_state: T, move_: T::Move) -> Self {
        MctsNode {
            unexpanded_moves: game_state.available_moves(),
            move_,
            game_state,
            score: 0.,
            visits: 0,
        }
    }

    #[inline]
    fn visits_f(&self) -> f32 {
        self.visits as f32
    }
}

#[cfg(test)]
mod tests {
    use std::{f32::consts::FRAC_1_SQRT_2, time::Duration};

    use rand::Rng;
    use rstest::*;

    use super::*;

    type MockGameState = i32;

    impl GameState for MockGameState {
        type Move = i32;
        type Player = bool;
        type MoveIterator = std::vec::IntoIter<Self::Move>;

        fn initial_state() -> Self {
            0
        }

        fn available_moves(&self) -> Self::MoveIterator {
            if self.terminal_value(self.next_to_play()).is_some() {
                vec![].into_iter()
            } else {
                vec![1, 3].into_iter()
            }
        }

        fn next_to_play(&self) -> Self::Player {
            // `true` plays on even numbers, `false` plays on odd
            self % 2 == 0
        }

        fn apply_move(&self, move_: &Self::Move) -> Self {
            self + move_
        }

        fn terminal_value(&self, for_player: Self::Player) -> Option<f32> {
            let score = if for_player == self.next_to_play() {
                *self as f32
            } else {
                -*self as f32
            };

            if score.abs() >= 10. {
                Some(score)
            } else {
                None
            }
        }
    }

    #[fixture]
    fn searcher() -> Searcher<MockGameState> {
        Searcher::new(SearchParameters {
            exploration_factor: FRAC_1_SQRT_2,
            search_iterations: 20,
        })
    }

    fn random_node(
        searcher: &mut Searcher<MockGameState>,
        parent: Option<NodeId>,
    ) -> (MockGameState, NodeId) {
        let state: MockGameState = rand::thread_rng().gen_range(0..10);
        let node_id: NodeId;

        if let Some(parent_id) = parent {
            let move_ = searcher.node(parent_id).game_state - state;
            node_id = searcher.arena.new_node(MctsNode::new(state, move_));
            parent_id.append(node_id, &mut searcher.arena);
        } else {
            node_id = searcher.arena.new_node(MctsNode::new(state, 0));
        }

        (state, node_id)
    }

    fn random_node_with_mcts_data(
        searcher: &mut Searcher<MockGameState>,
        parent: Option<NodeId>,
    ) -> (MockGameState, NodeId) {
        let game_state: MockGameState = rand::thread_rng().gen_range(0..10);
        let score = rand::thread_rng().gen_range(-12.0..12.0);
        let visits = rand::thread_rng().gen_range(1..100);
        let unexpanded_moves = game_state.available_moves();

        let node_id: NodeId;

        if let Some(parent_id) = parent {
            let move_ = searcher.node(parent_id).game_state - game_state;
            let node = MctsNode {
                game_state,
                move_,
                score,
                visits,
                unexpanded_moves,
            };
            node_id = searcher.arena.new_node(node);
            parent_id.append(node_id, &mut searcher.arena);
        } else {
            let node = MctsNode {
                game_state,
                move_: 0,
                score,
                visits,
                unexpanded_moves,
            };
            node_id = searcher.arena.new_node(node);
        }

        (game_state, node_id)
    }

    fn consume_unexpanded_moves(searcher: &mut Searcher<MockGameState>, node_id: NodeId) {
        let moves = &mut searcher.node_mut(node_id).unexpanded_moves;
        moves.for_each(drop);
    }

    #[rstest]
    fn starting_tree_creates_new_tree(mut searcher: Searcher<MockGameState>) {
        let state: MockGameState = rand::random();
        let starting_tree = searcher.starting_tree(state);

        assert_eq!(searcher.node(starting_tree).game_state, state);
    }

    #[rstest]
    fn starting_tree_finds_old_tree_and_detaches(mut searcher: Searcher<MockGameState>) {
        // FIXME: have seen this test fail once, don't know what caused it. probably
        // worth looking into property testing to find the failure state
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

    #[rstest]
    fn starting_tree_creates_new_if_children_dont_match(mut searcher: Searcher<MockGameState>) {
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

    #[rstest]
    fn best_child_maximizes_ucb1(mut searcher: Searcher<MockGameState>) {
        // arbitrary values chosen so that child a has a higher UCB1 when
        // effective_exploration_factor is set to 0, and child b is higher when
        // effective_exploration_factor is set to 1
        let mut parent: MctsNode<MockGameState> = MctsNode::new(rand::random(), 0);
        parent.score = -1.;
        parent.visits = 3;

        let child_a_state = rand::random();
        let child_a_move = parent.game_state - child_a_state;
        let mut child_a: MctsNode<MockGameState> = MctsNode::new(child_a_state, child_a_move);
        child_a.score = 5.;
        child_a.visits = 50;

        let child_b_state = rand::random();
        let child_b_move = parent.game_state - child_b_state;
        let mut child_b: MctsNode<MockGameState> = MctsNode::new(child_b_state, child_b_move);
        child_b.score = 1.;
        child_b.visits = 20;

        let node_parent = searcher.arena.new_node(parent);

        let node_child_a = searcher.arena.new_node(child_a);
        node_parent.append(node_child_a, &mut searcher.arena);
        let node_child_b = searcher.arena.new_node(child_b);
        node_parent.append(node_child_b, &mut searcher.arena);

        let best_child_c_0 = searcher.best_child(node_parent, 0.);
        let best_child_c_1 = searcher.best_child(node_parent, 1.);

        assert_eq!(best_child_c_0, node_child_a);
        assert_eq!(best_child_c_1, node_child_b);
    }

    #[rstest]
    #[timeout(Duration::from_secs(1))]
    fn rollout_terminates() {
        let terminal_value = Searcher::rollout(&MockGameState::initial_state(), true);
        assert!(terminal_value.abs() >= 10.);
    }

    #[rstest]
    fn backup_negamax_backs_up(mut searcher: Searcher<MockGameState>) {
        let node1 = random_node(&mut searcher, None);
        let node2 = random_node(&mut searcher, Some(node1.1));
        let node3 = random_node(&mut searcher, Some(node2.1));
        let node4 = random_node(&mut searcher, Some(node3.1));

        let score: f32 = rand::random();
        searcher.backup_negamax(node4.1, score);

        let get_score = |n: (MockGameState, NodeId)| searcher.node(n.1).score;

        assert_eq!(get_score(node4), score);
        assert_eq!(get_score(node3), -score);
        assert_eq!(get_score(node2), score);
        assert_eq!(get_score(node1), -score);
    }

    #[rstest]
    fn backup_negamax_avoids_unrelated_nodes(mut searcher: Searcher<MockGameState>) {
        let node1 = random_node(&mut searcher, None);
        let node2 = random_node(&mut searcher, Some(node1.1));
        let node3 = random_node(&mut searcher, Some(node2.1));
        let node4 = random_node(&mut searcher, Some(node3.1));

        let node_a = random_node(&mut searcher, Some(node1.1));
        let node_a1 = random_node(&mut searcher, Some(node_a.1));
        let node_b = random_node(&mut searcher, Some(node2.1));
        let node_c = random_node(&mut searcher, Some(node3.1));
        let node_d = random_node(&mut searcher, Some(node4.1));

        searcher.backup_negamax(node4.1, rand::random());

        let get_score = |n: (MockGameState, NodeId)| searcher.node(n.1).score;

        assert_eq!(get_score(node_a), 0.);
        assert_eq!(get_score(node_a1), 0.);
        assert_eq!(get_score(node_b), 0.);
        assert_eq!(get_score(node_c), 0.);
        assert_eq!(get_score(node_d), 0.);
    }

    #[rstest]
    fn expand_finds_node_to_expand(mut searcher: Searcher<MockGameState>) {
        let node = random_node(&mut searcher, None);

        let expanded = searcher.expand(node.1);

        assert!(expanded.is_some());
        assert_ne!(expanded.unwrap(), node.1);
    }

    #[rstest]
    fn expand_returns_none_if_node_is_fully_expanded(mut searcher: Searcher<MockGameState>) {
        let node = random_node(&mut searcher, None);

        consume_unexpanded_moves(&mut searcher, node.1);

        let expanded = searcher.expand(node.1);

        assert!(expanded.is_none());
    }

    #[rstest]
    fn tree_policy_can_expand(mut searcher: Searcher<MockGameState>) {
        let node_1 = random_node(&mut searcher, None);

        let tree_policy_result = searcher.tree_policy(node_1.1);

        let mut children = tree_policy_result.children(&searcher.arena);
        assert!(children.next().is_none());

        let mut ancestors = tree_policy_result.ancestors(&searcher.arena).skip(1);
        assert!(ancestors.next().is_some());
    }

    #[rstest]
    fn tree_policy_finds_leaf(mut searcher: Searcher<MockGameState>) {
        let node_1 = random_node_with_mcts_data(&mut searcher, None);
        let node_1_1 = random_node_with_mcts_data(&mut searcher, Some(node_1.1));
        let node_1_2 = random_node_with_mcts_data(&mut searcher, Some(node_1.1));
        let _leaf_1_1_1 = random_node_with_mcts_data(&mut searcher, Some(node_1_1.1));
        let _leaf_1_1_2 = random_node_with_mcts_data(&mut searcher, Some(node_1_1.1));
        let _leaf_1_2_1 = random_node_with_mcts_data(&mut searcher, Some(node_1_2.1));

        consume_unexpanded_moves(&mut searcher, node_1.1);
        consume_unexpanded_moves(&mut searcher, node_1_1.1);
        consume_unexpanded_moves(&mut searcher, node_1_2.1);

        let tree_policy_result = searcher.tree_policy(node_1.1);

        let mut children = tree_policy_result.children(&searcher.arena);
        assert!(children.next().is_none());

        let mut ancestors = tree_policy_result.ancestors(&searcher.arena).skip(1);
        assert!(ancestors.next().is_some());
    }

    #[rstest]
    fn tree_policy_stops_if_given_terminal_state(mut searcher: Searcher<MockGameState>) {
        let mut data = MctsNode::new(10, 0);
        data.unexpanded_moves = vec![0].into_iter();
        let node = searcher.arena.new_node(data);

        let leaf = searcher.tree_policy(node);

        assert_eq!(node, leaf);

        let mcts_node = searcher.node_mut(leaf);
        assert_eq!(mcts_node.unexpanded_moves.next(), Some(0));
        assert_eq!(mcts_node.unexpanded_moves.next(), None);
    }

    #[rstest]
    fn tree_policy_stops_if_best_child_is_terminal_state(mut searcher: Searcher<MockGameState>) {
        searcher.parameters.exploration_factor = 0.0;

        let parent = random_node_with_mcts_data(&mut searcher, None);
        consume_unexpanded_moves(&mut searcher, parent.1);

        // "worst" by UCB1
        let mut worst_state = MctsNode::new(0, 0);
        worst_state.visits = 1;
        let worst_node = searcher.arena.new_node(worst_state);
        parent.1.append(worst_node, &mut searcher.arena);

        let terminal_state = MctsNode {
            game_state: 10,
            move_: 10,
            unexpanded_moves: vec![0].into_iter(),
            visits: 1,
            score: 1.0,
        };
        let terminal_node = searcher.arena.new_node(terminal_state);
        parent.1.append(terminal_node, &mut searcher.arena);

        let leaf = searcher.tree_policy(parent.1);

        assert_eq!(leaf, terminal_node);

        let mcts_node = searcher.node_mut(leaf);
        assert_eq!(mcts_node.unexpanded_moves.next(), Some(0));
        assert_eq!(mcts_node.unexpanded_moves.next(), None);
    }

    #[rstest]
    #[timeout(Duration::from_secs(1))]
    pub fn search_terminates(mut searcher: Searcher<MockGameState>) {
        searcher.search(MockGameState::initial_state());
    }

    #[rstest]
    pub fn search_returns_legal_move(mut searcher: Searcher<MockGameState>) {
        let move_ = searcher.search(MockGameState::initial_state());

        let legal = MockGameState::initial_state()
            .available_moves()
            .any(|m| m == move_);
        assert!(legal);
    }

    #[rstest]
    pub fn search_remembers_something(mut searcher: Searcher<MockGameState>) {
        searcher.search(MockGameState::initial_state());
        assert!(searcher.previous_choice.is_some());
    }

    #[rstest]
    pub fn search_remembers_correct_choice(mut searcher: Searcher<MockGameState>) {
        let move_ = searcher.search(MockGameState::initial_state());

        let chosen = searcher.node(searcher.previous_choice.unwrap());
        let returned_game_state = MockGameState::initial_state().apply_move(&move_);

        assert_eq!(chosen.game_state, returned_game_state);
    }
}
