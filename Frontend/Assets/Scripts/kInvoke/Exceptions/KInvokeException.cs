using System;

namespace kInvoke.Exceptions
{
    public class KInvokeException : Exception
    {
        public KInvokeException()
        {
        }

        public KInvokeException(string message) : base(message)
        {
        }

        public KInvokeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
