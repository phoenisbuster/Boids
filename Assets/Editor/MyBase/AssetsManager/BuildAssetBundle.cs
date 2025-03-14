using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyBase.Editor.AssetsManager
{
    public class BuildAssetBundles
    {
        private static string[] buildOptions = new[]
        {
            "StreamingAssets/AssetBundles (Default)",
            "Application.dataPath/AssetBundles"
        };

        private static string GetBuildPath(int optionIndex)
        {
            switch (optionIndex)
            {
                case 0: // StreamingAssets
                    return Path.Combine(Application.streamingAssetsPath, "AssetBundles");
                case 1: // Application.dataPath
                    return Path.Combine(Application.dataPath, "AssetBundles");
                default:
                    return Path.Combine(Application.streamingAssetsPath, "AssetBundles"); // Default to StreamingAssets
            }
        }
        
        [MenuItem("MyBase/Assets/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            int choice = EditorUtility.DisplayDialogComplex(
                "Choose AssetBundle Build Directory",
                "Select where to build the AssetBundles:",
                buildOptions[0], // Default option
                "Cancel",
                buildOptions[1]
            );
            
            string outputPath = "Assets/StreamingAssets/AssetBundles";
            if (!System.IO.Directory.Exists(outputPath)) System.IO.Directory.CreateDirectory(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}

