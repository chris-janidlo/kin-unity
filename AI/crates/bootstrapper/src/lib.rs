use std::{
    env::current_dir,
    io::Read,
    os::windows::process::CommandExt,
    process::{Child, Command, Stdio},
};

// TODO: make a custom panic handler that doesn't crash Unity
// TODO: add a kill_server function to be called when exiting Play Mode

/// Launches an `ai_server` process, returning the port number the process listens on.
#[no_mangle]
pub extern "C" fn launch_server() -> i32 {
    let mut server_path = current_dir().expect("should be able to access current directory");
    server_path.push("ai_server");

    // for Windows: prevent new window from opening
    const CREATE_NO_WINDOW: u32 = 0x08000000;

    let server_proc = Command::new(server_path)
        .creation_flags(CREATE_NO_WINDOW)
        .stdout(Stdio::piped())
        .spawn()
        .expect("should be able to launch process");

    read_port_number(server_proc)
}

/// Reads server_proc's stdout to determine what port it's listening on.
fn read_port_number(server_proc: Child) -> i32 {
    // server always pads port output to 5 places
    let mut buf = [0; 5];

    server_proc
        .stdout
        .expect("stdout should be set up and readable")
        .read_exact(&mut buf)
        .expect("should be able to read from stdout");

    std::str::from_utf8(&buf)
        .expect("stdout should be utf8 encoded")
        .parse::<i32>()
        .expect("message should be in integer form")
}
