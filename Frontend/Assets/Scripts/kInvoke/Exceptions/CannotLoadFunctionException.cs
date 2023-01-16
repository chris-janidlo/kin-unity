using System;

namespace kInvoke.Exceptions
{
    public class CannotLoadFunctionException : KInvokeException
    {
        public CannotLoadFunctionException() { }

        public CannotLoadFunctionException(string message)
            : base(message) { }

        public CannotLoadFunctionException(string message, Exception inner)
            : base(message, inner) { }
    }
}
