using System;
using System.Linq;
using kInvoke.Exceptions;

namespace kInvoke
{
    public static class LibraryFunction<TDelegate>
        where TDelegate : Delegate
    {
        private static TDelegate _cache;

        public static TDelegate Invoke
        {
            get
            {
                if (_cache != null) return _cache;

                LibraryImportAttribute attribute;
                try
                {
                    attribute = (LibraryImportAttribute)typeof(TDelegate)
                        .GetCustomAttributes(typeof(LibraryImportAttribute), false)
                        .Single();
                }
                catch (InvalidOperationException)
                {
                    var message =
                        $"must supply a {nameof(LibraryImportAttribute)} " +
                        $"in order to call {typeof(TDelegate).Name}";

                    throw new MissingLibraryImportAttribute(message);
                }

                var library = LibraryLoader.GetLibraryPointer(attribute.LibraryName);
                return _cache = LibraryLoader.GetNativeFunction<TDelegate>(library);
            }
        }
    }
}