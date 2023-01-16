using System;

namespace kInvoke.Exceptions
{
    public class CannotLoadLibraryException : KInvokeException
    {
        public CannotLoadLibraryException() { }

        public CannotLoadLibraryException(string message)
            : base(message) { }

        public CannotLoadLibraryException(string message, Exception inner)
            : base(message, inner) { }
    }
}
