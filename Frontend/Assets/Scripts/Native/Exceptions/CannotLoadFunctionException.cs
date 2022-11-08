using System;

namespace Native.Exceptions
{
    public class CannotLoadFunctionException : NativeException
    {
        public CannotLoadFunctionException()
        {
        }

        public CannotLoadFunctionException(string message) : base(message)
        {
        }

        public CannotLoadFunctionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}