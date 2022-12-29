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
    pub fn new(parameters: SearchParameters) -> Self {
        Searcher {
            arena: Arena::new(),
            previous_choice: None,
            parameters,
        }
    }

    pub fn search(&mut self, _starting_state: T, _parameters: SearchParameters) -> T::Move {
        todo!()
        // create root node v_0 with state s_0 [call starting_tree]
        // while within computational budget [will take iterations as parameter]
        //		v_1 = tree_policy(v_0)
        //		score = rollout(v_1) [rollout will repeatadly call default_policy method on GameState]
        //		backup(score, v_1)
        // return best_child
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

    /// effective_exploration_factor is also known as c (Browne et al 2012, p. 9)
    fn best_child(&self, parent: NodeId, effective_exploration_factor: f32) -> NodeId {
        let ucb1 = |id: &NodeId| {
            let parent = self.node(parent);
            let child = self.node(*id);

            let exploitation_term = child.score / child.visits_f();
            let exploration_term = (2. * parent.visits_f().ln() / child.visits_f()).sqrt();

            exploitation_term + effective_exploration_factor * exploration_term
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
}

impl<T> MctsNode<T>
where
    T: GameState,
{
    #[inline]
    pub fn visits_f(&self) -> f32 {
        self.visits as f32
    }
}

#[cfg(test)]
mod tests {
    use rstest::{fixture, rstest};

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

    #[fixture]
    fn searcher() -> Searcher<MockGameState> {
        Searcher::new(SearchParameters {
            exploration_factor: 1.,
            search_iterations: 20,
        })
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

    #[rstest]
    fn starting_tree_creates_new_tree(mut searcher: Searcher<MockGameState>) {
        let state: MockGameState = rand::random();
        let starting_tree = searcher.starting_tree(state);

        assert_eq!(searcher.node(starting_tree).game_state, state);
    }

    #[rstest]
    fn starting_tree_finds_old_tree_and_detaches(mut searcher: Searcher<MockGameState>) {
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
        let parent: MctsNode<MockGameState> = MctsNode {
            game_state: rand::random(),
            score: -1.,
            visits: 3,
        };
        let child_a: MctsNode<MockGameState> = MctsNode {
            game_state: rand::random(),
            score: 5.,
            visits: 50,
        };
        let child_b: MctsNode<MockGameState> = MctsNode {
            game_state: rand::random(),
            score: 1.,
            visits: 20,
        };

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
}
