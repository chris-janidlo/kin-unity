using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using kInvoke.Exceptions;
using UnityEngine;
using UnityEditor;

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
            if (LibraryPointerCache.TryGetValue(libraryName, out var libraryPointer)) return libraryPointer;

            libraryPointer = OpenLibraryCrossPlatform(libraryName);
            if (libraryPointer == IntPtr.Zero) throw new CannotLoadLibraryException(libraryName);

            return LibraryPointerCache[libraryName] = libraryPointer;
        }

        public static TDelegate GetNativeFunction<TDelegate>(IntPtr libraryPointer)
            where TDelegate : Delegate
        {
            var functionName = typeof(TDelegate).Name;

            if (!FunctionPointerCache.TryGetValue(functionName, out var functionPointer))
            {
                functionPointer = GetFunctionPointerCrossPlatform(libraryPointer, functionName);

                if (functionPointer == IntPtr.Zero) throw new CannotLoadFunctionException(functionName);
                FunctionPointerCache[functionName] = functionPointer;
            }

            return Marshal.GetDelegateForFunctionPointer<TDelegate>(functionPointer);
        }

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
        [DllImport("__Internal")]
        private static extern IntPtr dlopen(string path, int flag);
    
        private static IntPtr OpenLibraryCrossPlatform(string path)
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            var extension = path.Split(".").Last();
            if (extension != "bundle")
            {
                path += ".bundle";
            }
#endif
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            // take advantage of the fact that `dlopen` searches the current working directory, which for macOS apps
            //  should be the directory containing the running app
            // given that, modify the path passed to `dlopen` to point inside the app bundle
            // FIXME: there's probably a better way to get the name of the running app
            var appName = Environment.CommandLine
                .Split("/")
                .Last(s => s.Contains(".app"));
            path = $"{appName}/{path}";
#endif
            Debug.Log($"{nameof(OpenLibraryCrossPlatform)}, macOS: opening library at path {path}");
            return dlopen(path, 0);
        }

        [DllImport("__Internal")]
        private static extern int dlclose(IntPtr handle);

        private static void CloseLibraryCrossPlatform(IntPtr pointer)
        {
            dlclose(pointer);
        }
    
        [DllImport("__Internal")]
        private static extern IntPtr dlsym(IntPtr handle, string symbolName);

        private static IntPtr GetFunctionPointerCrossPlatform(IntPtr libraryPointer, string name)
        {
            return dlsym(libraryPointer, name);
        }
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);
    
        private static IntPtr OpenLibraryCrossPlatform(string path)
        {
            return LoadLibrary(path);
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        private static void CloseLibraryCrossPlatform(IntPtr pointer)
        {
            FreeLibrary(pointer);
        }

        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        private static IntPtr GetFunctionPointerCrossPlatform(IntPtr libraryPointer, string name)
        {
            return GetProcAddress(libraryPointer, name);
        }
#else
        #error "Unsupported platform"
#endif

#if UNITY_EDITOR
        private static void Teardown(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange != PlayModeStateChange.ExitingPlayMode) return;

            foreach (var pointer in LibraryPointerCache.Values)
                CloseLibraryCrossPlatform(pointer);
            EditorApplication.playModeStateChanged -= Teardown;
        }
#endif // UNITY_EDITOR
    }
}