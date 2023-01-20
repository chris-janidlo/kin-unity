use std::net::*;

use anyhow::Result;
use log::*;
use serde_json::{Deserializer, Value};

const SERVER_IP: IpAddr = IpAddr::V4(Ipv4Addr::new(127, 0, 0, 1));
const PORT: u16 = 1370; // hard coded for now. eventually want to grab a random open port and return that to the parent process

pub fn start() -> Result<()> {
    let addr = SocketAddr::new(SERVER_IP, PORT);

    info!("listening on {addr}");

    let listener = TcpListener::bind(addr)?;

    for stream in listener.incoming() {
        info!("got connection!");
        echo(stream?)?;
        info!("end connection handling");
    }

    Ok(())
}

fn echo(stream: TcpStream) -> Result<()> {
    let de = Deserializer::from_reader(stream);

    for value in de.into_iter::<Value>() {
        info!("got value!");
        info!("{}", value?);
        info!("end value handling");
    }

    Ok(())
}
