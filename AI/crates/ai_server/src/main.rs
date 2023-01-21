mod game_state;
mod mcts;
mod rules;
mod server;

use anyhow::Result;

pub fn main() -> Result<()> {
    server::start()?;

    Ok(())
}
