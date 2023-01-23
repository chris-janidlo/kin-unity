using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

public class AiServer : IDisposable
{
    // note that my first choice when implementing this was a finalizer, to not require
    // manual closing. however, the current implementation of `bootstrapper` uses
    // thread-local memory to store server PIDs, and finalizers are called in a separate
    // thread, so the `bootstrapper` call in the finalizer wouldn't see the server
    // process as existing and would just return without erroring or closing anything.
    // that probably also removes SafeHandle from contention

    [DllImport("bootstrapper")]
    private static extern int open_server();

    [DllImport("bootstrapper")]
    private static extern int get_tcp_port(int pid);

    [DllImport("bootstrapper")]
    private static extern int close_server(int pid);

    private int pid;
    private int? port;

    public int Port
    {
        get
        {
            if (port.HasValue)
                return port.Value;

            var result = get_tcp_port(pid);

            if (result < 0)
                throw new AiServerException(nameof(get_tcp_port), result);

            port = result;
            return result;
        }
    }

    public AiServer()
    {
        var result = open_server();

        if (result < 0)
            throw new AiServerException(nameof(open_server), result);

        pid = result;
    }

    public void Dispose()
    {
        var code = close_server(pid);

        if (code < 0)
            throw new AiServerException(nameof(close_server), code);
    }

    public void IntentionallyError()
    {
        var code = close_server(-69);

        if (code < 0)
            throw new AiServerException(nameof(close_server), code);
    }
}

public class AiServerException : Exception
{
    public AiServerException(string methodName, int code)
        : base(FormatMessage(methodName, code)) { }

    public AiServerException(string methodName, int code, Exception inner)
        : base(FormatMessage(methodName, code), inner) { }

    public static string FormatMessage(string methodName, int code)
    {
        return $"Error when calling {methodName}: {code}";
    }
}
