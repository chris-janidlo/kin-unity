using System;
using System.Runtime.InteropServices;

namespace Code.Player.AIComms
{
    public class AiServer : IDisposable
    {
        private readonly int pid;
        private int? port;

        public AiServer()
        {
            int result = open_server();

            if (result < 0)
                throw new AiServerException(nameof(open_server), result);

            pid = result;
        }

        public int Port
        {
            get
            {
                if (port.HasValue)
                    return port.Value;

                int result = get_tcp_port(pid);

                if (result < 0)
                    throw new AiServerException(nameof(get_tcp_port), result);

                port = result;
                return result;
            }
        }

        public void Dispose()
        {
            int code = close_server(pid);

            if (code < 0)
                throw new AiServerException(nameof(close_server), code);
        }

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

        public void IntentionallyError()
        {
            int code = close_server(-69);

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
}
