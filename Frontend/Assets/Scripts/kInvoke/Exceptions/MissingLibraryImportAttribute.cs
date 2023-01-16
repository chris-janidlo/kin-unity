using System;

namespace kInvoke.Exceptions
{
    public class MissingLibraryImportAttribute : KInvokeException
    {
        public MissingLibraryImportAttribute() { }

        public MissingLibraryImportAttribute(string message)
            : base(message) { }

        public MissingLibraryImportAttribute(string message, Exception inner)
            : base(message, inner) { }
    }
}
