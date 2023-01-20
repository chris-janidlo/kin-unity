mod game_state;
mod mcts;
mod rules;
mod server;

use anyhow::Result;
use env_logger;

pub fn main() -> Result<()> {
    env_logger::init();
    server::start()?;

    Ok(())
}
