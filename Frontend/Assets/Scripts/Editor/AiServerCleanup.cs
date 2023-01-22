using System.Runtime.InteropServices;
using UnityEditor;

/// <summary>
/// Closes any open AI server processes on exiting Play Mode, since consumers generally
/// aren't expected to do so.
/// <summary>
[InitializeOnLoad]
public static class AiServerCleanup
{
    [DllImport("bootstrapper")]
    private static extern void close_all();

    static AiServerCleanup()
    {
        EditorApplication.playModeStateChanged += KillServer;
    }

    public static void KillServer(PlayModeStateChange newState)
    {
        if (newState != PlayModeStateChange.EnteredEditMode)
            return;

        close_all();
    }
}
