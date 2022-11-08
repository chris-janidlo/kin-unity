using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using crass;
using Native.Exceptions;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
// TODO: https://www.jacksondunstan.com/articles/3945
namespace Native
{
    // only supports one native plugin, KinAI, but can easily be extended to support others
    public class Plugin : Singleton<Plugin>
    {
        private const string LibraryName = "KinAI";

        private readonly Dictionary<string, IntPtr> _functionPointerCache = new();
        private IntPtr _libraryPointer;

        private void Awake()
        {
            SingletonSetPersistantInstance(this);
        }

        private void OnDestroy()
        {
            if (_libraryPointer != IntPtr.Zero) FreeLibrary(_libraryPointer);
        }

        // TODO: this is not type safe at all
        // is there a way to subclass Delegate into say NativeDelegate such that calling subclasses of NativeDelegate
        // calls into dynamically-loaded libraries like this? (need to research C# 9 function pointers)
        public void Invoke<TDelegate>(params object[] parameters)
            where TDelegate : Delegate
        {
            var function = GetNativeFunction<TDelegate>();
            function.DynamicInvoke(parameters);
        }

        public TReturn Invoke<TDelegate, TReturn>(params object[] parameters)
            where TDelegate : Delegate
        {
            var function = GetNativeFunction<TDelegate>();
            return (TReturn)function.DynamicInvoke(parameters);
        }

        private static void AssertInstance()
        {
            if (Instance == null)
                throw new InvalidOperationException(
                    $"no instance of {nameof(Plugin)} found. did you forget to add the prefab to the scene?"
                );
        }

        private IntPtr GetLibraryPointer()
        {
            if (_libraryPointer == IntPtr.Zero)
            {
                _libraryPointer = LoadLibrary(LibraryName);
                if (_libraryPointer == IntPtr.Zero) throw new CannotLoadLibraryException(LibraryName);
            }

            return _libraryPointer;
        }

        private Delegate GetNativeFunction<TDelegate>()
        {
            // get library pointer first to fail fast
            var libraryPointer = GetLibraryPointer();

            var functionName = typeof(TDelegate).Name;
            if (!_functionPointerCache.TryGetValue(functionName, out var functionPointer))
            {
                // TODO: I think this is safe to cache but want to do more research to be sure
                functionPointer = GetProcAddress(libraryPointer, functionName);
                if (functionPointer == IntPtr.Zero) throw new CannotLoadFunctionException(functionName);
            }

            return Marshal.GetDelegateForFunctionPointer(functionPointer, typeof(TDelegate));
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
    }
}
#endif