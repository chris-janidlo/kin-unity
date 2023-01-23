#[cfg(windows)]
use std::os::windows::process::CommandExt;
use std::{
    cell::RefCell,
    collections::HashMap,
    env::current_exe,
    fs,
    io::Read,
    path::PathBuf,
    process::{Child, Command, Stdio},
};

// TODO: better error handling - log some human readable info in a file somwhere, and only then panic

thread_local! {
    static PROC_MAP: RefCell<HashMap<u32, Child>> = RefCell::new(Default::default());
}

/// Launches an `ai_server` process, returning its PID.
#[no_mangle]
pub extern "C" fn open_server() -> u32 {
    let mut command = Command::new(get_server_path());
    command.stdout(Stdio::piped());

    #[cfg(windows)]
    {
        const CREATE_NO_WINDOW: u32 = 0x08000000;
        command.creation_flags(CREATE_NO_WINDOW);
    }

    let server_proc = command.spawn().expect("should be able to launch process");

    let pid = server_proc.id();

    PROC_MAP.with(|map| map.borrow_mut().insert(pid, server_proc));

    pid
}

/// Retrieves the TCP port from an `ai_server` process, as delimited by PID.
// TODO: allow consumers to call this more than once for the same PID without crashing?
#[no_mangle]
pub extern "C" fn get_tcp_port(pid: u32) -> u32 {
    let mut port: u32 = 0;

    PROC_MAP.with(|map| {
        if let Some(proc) = map.borrow_mut().get_mut(&pid) {
            let mut stream = proc
                .stdout
                .take()
                .expect("stdout should be set up and readable");

            port = read_port_number(&mut stream);
        } else {
            panic!("PID should be valid");
        }
    });

    port
}

/// Closes a given `ai_server` process by PID.
#[no_mangle]
pub extern "C" fn close_server(pid: u32) {
    PROC_MAP.with(|map| {
        if let Some(mut proc) = map.borrow_mut().remove(&pid) {
            proc.kill().expect("should be able to kill this process");
        }
    });
}

/// Close every open `ai_server` process that was spawned from this thread.
#[no_mangle]
pub extern "C" fn close_all() {
    PROC_MAP.with(|map| {
        for (_, mut proc) in map.borrow_mut().drain() {
            proc.kill().expect("should be able to kill this process");
        }
    })
}

/// Reads a port number from a stream that only has a port number to read.
///
/// # Arguments
///
/// * `stream` - Any readable stream that has a port number. Is expected to have exactly
///   5 bytes of data to read, with left-padded 0s as necessary.
fn read_port_number(stream: &mut impl Read) -> u32 {
    let mut buf = [0; 5];

    stream
        .read_exact(&mut buf)
        .expect("should be able to read from stream");

    std::str::from_utf8(&buf)
        .expect("stream should be utf8 encoded")
        .parse::<u32>()
        .expect("message should be in integer form")
}

fn get_server_path() -> PathBuf {
    let finding = fs::read_dir("./")
        .expect("should be able to read current directory")
        .find_map(|dir_entry| {
            let de = dir_entry.expect("should be able to inspect dir_entry");

            if de.file_name() == "ai_server" {
                Some(de.path())
            } else {
                None
            }
        });

    if let Some(path) = finding {
        path
    } else if cfg!(macos) {
        let mut path = current_exe().expect("should be able to get running macOS app");
        path.push("ai_server");
        path
    } else {
        panic!("can't find ai_server")
    }
}
