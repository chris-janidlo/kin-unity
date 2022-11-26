using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;

namespace Editor
{
    public static class BuildScripts
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.StandaloneOSX) return;

            const string filename = "KinAI.bundle";
            
            var projectDirectory = Directory.GetParent(Application.dataPath)!.FullName;
            var sourcePath = Path.Combine(projectDirectory, filename);
            var destPath = Path.Combine(pathToBuiltProject, filename);
            
            Debug.Log($"Copying {sourcePath} to {destPath}");
            File.Copy(sourcePath, destPath, true);
        }
    }
}
