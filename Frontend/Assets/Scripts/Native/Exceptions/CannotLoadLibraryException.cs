using System;

namespace Native.Exceptions
{
    public class CannotLoadLibraryException : NativeException
    {
        public CannotLoadLibraryException()
        {
        }

        public CannotLoadLibraryException(string message) : base(message)
        {
        }

        public CannotLoadLibraryException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}