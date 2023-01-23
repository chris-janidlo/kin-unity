#[cfg(windows)]
use std::os::windows::process::CommandExt;
use std::{
    cell::RefCell,
    collections::HashMap,
    env::{current_dir, current_exe},
    fs::{read_dir, File},
    io::{Read, Write},
    path::PathBuf,
    process::{Child, Command, Stdio},
    result::Result::{Err, Ok},
    time::{SystemTime, UNIX_EPOCH},
};

use anyhow::*;

thread_local! {
    static PROC_MAP: RefCell<HashMap<u32, Child>> = RefCell::new(Default::default());
}

/// Launches an `ai_server` process, returning its PID.
#[no_mangle]
pub extern "C" fn open_server() -> i64 {
    match open_server_impl() {
        Ok(o) => o as i64,
        Err(e) => dump(e),
    }
}

/// Retrieves the TCP port from an `ai_server` process, as delimited by PID.
// TODO: allow consumers to call this more than once for the same PID without crashing?
#[no_mangle]
pub extern "C" fn get_tcp_port(pid: u32) -> i64 {
    match get_tcp_port_impl(pid) {
        Ok(o) => o as i64,
        Err(e) => dump(e),
    }
}

/// Closes a given `ai_server` process by PID.
#[no_mangle]
pub extern "C" fn close_server(pid: u32) -> i64 {
    match close_server_impl(pid) {
        Ok(_) => 1,
        Err(e) => dump(e),
    }
}

/// Close every open `ai_server` process that was spawned from this thread.
#[no_mangle]
pub extern "C" fn close_all() -> i64 {
    match close_all_impl() {
        Ok(_) => 1,
        Err(e) => dump(e),
    }
}

fn dump(err: Error) -> i64 {
    // since we're in an error handling function, give up without complaint if anything goes wrong
    if let Ok(mut path) = current_dir() {
        if let Ok(duration) = SystemTime::now().duration_since(UNIX_EPOCH) {
            let timestamp = duration.as_secs();
            let filename = format!("ai_bootstrap_crash_{timestamp}.log");

            path.push(filename);

            if let Ok(mut file) = File::create(path) {
                // use _ to ignore result
                let _ = write!(file, "{err}");
            }
        }
    }

    -1
}

fn open_server_impl() -> Result<u32> {
    let mut command = Command::new(get_server_path()?);
    command.stdout(Stdio::piped());

    #[cfg(windows)]
    {
        const CREATE_NO_WINDOW: u32 = 0x08000000;
        command.creation_flags(CREATE_NO_WINDOW);
    }

    let server_proc = command
        .spawn()
        .context("should be able to launch ai_server")?;

    let pid = server_proc.id();

    PROC_MAP.with(|map| map.borrow_mut().insert(pid, server_proc));

    Ok(pid)
}

fn get_tcp_port_impl(pid: u32) -> Result<u32> {
    PROC_MAP.with(|map| {
        if let Some(proc) = map.borrow_mut().get_mut(&pid) {
            let mut stream = proc
                .stdout
                .take()
                .context("stdout should be set up and readable")?;

            read_port_number(&mut stream)
        } else {
            Err(anyhow!("PID should be valid"))
        }
    })
}

fn close_server_impl(pid: u32) -> Result<()> {
    PROC_MAP.with(|map| {
        if let Some(mut proc) = map.borrow_mut().remove(&pid) {
            proc.kill().context("should be able to kill this process")
        } else {
            Err(anyhow!("PID {pid} is not known to this thread"))
        }
    })
}

fn close_all_impl() -> Result<()> {
    PROC_MAP.with(|map| {
        for (_, mut proc) in map.borrow_mut().drain() {
            proc.kill().context("should be able to kill this process")?;
        }

        Ok(())
    })
}

/// Reads a port number from a stream that only has a port number to read.
///
/// # Arguments
///
/// * `stream` - Any readable stream that has a port number. Is expected to have exactly
///   5 bytes of data to read, with left-padded 0s as necessary.
fn read_port_number(stream: &mut impl Read) -> Result<u32> {
    let mut buf = [0; 5];

    stream
        .read_exact(&mut buf)
        .context("should be able to read from stream")?;

    std::str::from_utf8(&buf)
        .context("stream should be utf8 encoded")?
        .parse::<u32>()
        .context("message should be in integer form")
}

fn get_server_path() -> Result<PathBuf> {
    let paths = read_dir("./").context("should be able to read current directory")?;

    for possible_entry in paths {
        let entry =
            possible_entry.context("intermittent IO error while reading current directory")?;

        if entry.file_name() == "ai_server" {
            return Ok(entry.path());
        }
    }

    if cfg!(macos) {
        let mut path = current_exe().context("should be able to get running macOS app")?;
        path.push("ai_server");
        Ok(path)
    } else {
        Err(anyhow!("can't find ai_server"))
    }
}
