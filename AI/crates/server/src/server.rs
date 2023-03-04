use std::{io::Write, net::*};

use anyhow::Result;
use serde_json::{Deserializer, Value};

pub fn start() -> Result<()> {
    let addr = SocketAddrV4::new(Ipv4Addr::LOCALHOST, 0);

    let listener = TcpListener::bind(addr)?;
    let port = listener.local_addr()?.port();

    // write to stdout so that Unity can read the port. padded to 5 places for
    // convenience
    println!("{port:05}");

    for stream in listener.incoming() {
        handle(stream?)?;
    }

    Ok(())
}

fn handle(stream: TcpStream) -> Result<()> {
    let mut clone = stream.try_clone()?;
    let de = Deserializer::from_reader(stream);

    let mut buf: [u8; 1] = [0];

    for _recvd in de.into_iter::<Value>() {
        buf[0] = rand::random();
        clone.write_all(&buf)?;
    }

    Ok(())
}
