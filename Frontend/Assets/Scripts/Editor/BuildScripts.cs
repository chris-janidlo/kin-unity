using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public static class BuildScripts
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    PostBuildWindows(pathToBuiltProject);
                    break;
                case BuildTarget.StandaloneOSX:
                    PostBuildMac(pathToBuiltProject);
                    break;
                default:
                    Debug.Log($"Unhandled {nameof(BuildTarget)}: {target}. No post-processing will occur");
                    break;
            }
        }

        private static void PostBuildWindows(string pathToBuiltProject)
        {
            const string filename = "KinAI.dll";

            var projectDirectory = Directory.GetParent(Application.dataPath)!.FullName;
            var buildDirectory = Directory.GetParent(pathToBuiltProject)!.FullName;

            var sourcePath = Path.Combine(projectDirectory, filename);
            var destPath = Path.Combine(buildDirectory, filename);

            Debug.Log($"Copying {sourcePath} to {destPath}");
            File.Copy(sourcePath, destPath, true);
        }

        private static void PostBuildMac(string pathToBuiltProject)
        {
            const string filename = "KinAI.bundle";

            var projectDirectory = Directory.GetParent(Application.dataPath)!.FullName;
            var sourcePath = Path.Combine(projectDirectory, filename);
            var destPath = Path.Combine(pathToBuiltProject, filename);

            Debug.Log($"Copying {sourcePath} to {destPath}");
            File.Copy(sourcePath, destPath, true);
        }
    }
}