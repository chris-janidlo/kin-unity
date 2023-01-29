mod server;

use anyhow::Result;

pub fn main() -> Result<()> {
    server::start()?;

    Ok(())
}
