using System;

namespace kInvoke
{
    [AttributeUsage(AttributeTargets.Delegate)]
    public class LibraryImportAttribute : Attribute
    {
        public string LibraryName;

        public LibraryImportAttribute(string libraryName)
        {
            LibraryName = libraryName;
        }
    }
}