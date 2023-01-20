use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct SearchParameters {
    pub exploration_factor: f32,
    pub search_iterations: i32,
}
