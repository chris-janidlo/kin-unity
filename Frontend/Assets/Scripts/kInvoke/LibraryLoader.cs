using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using kInvoke.Exceptions;
using UnityEditor;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
// TODO: https://www.jacksondunstan.com/articles/3945
namespace kInvoke
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif // UNITY_EDITOR
    internal static class LibraryLoader
    {
        // TODO: verify that these are safe to cache
        private static readonly Dictionary<string, IntPtr> FunctionPointerCache, LibraryPointerCache;

        static LibraryLoader()
        {
            FunctionPointerCache = new Dictionary<string, IntPtr>();
            LibraryPointerCache = new Dictionary<string, IntPtr>();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= Teardown;
            EditorApplication.playModeStateChanged += Teardown;
#endif // UNITY_EDITOR
        }

        public static IntPtr GetLibraryPointer(string libraryName)
        {
            if (!LibraryPointerCache.TryGetValue(libraryName, out var libraryPointer))
            {
                libraryPointer = LoadLibrary(libraryName);

                if (libraryPointer == IntPtr.Zero) throw new CannotLoadLibraryException(libraryName);
                LibraryPointerCache[libraryName] = libraryPointer;
            }

            return libraryPointer;
        }

        public static TDelegate GetNativeFunction<TDelegate>(IntPtr libraryPointer)
            where TDelegate : Delegate
        {
            var functionName = typeof(TDelegate).Name;

            if (!FunctionPointerCache.TryGetValue(functionName, out var functionPointer))
            {
                functionPointer = GetProcAddress(libraryPointer, functionName);

                if (functionPointer == IntPtr.Zero) throw new CannotLoadFunctionException(functionName);
                FunctionPointerCache[functionName] = functionPointer;
            }

            return Marshal.GetDelegateForFunctionPointer<TDelegate>(functionPointer);
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

#if UNITY_EDITOR
        private static void Teardown(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange != PlayModeStateChange.ExitingPlayMode) return;

            foreach (var pointer in LibraryPointerCache.Values)
                FreeLibrary(pointer);
            EditorApplication.playModeStateChanged -= Teardown;
        }
#endif // UNITY_EDITOR
    }
}
#endif // UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN