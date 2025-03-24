#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyBase.Editor.AssetsManager
{
    public class BuildAssetBundles
    {
        private static string[] buildOptions = new[]
        {
            "StreamingAssets",
            "Custom"
        };

        private static string GetBuildPath(int optionIndex)
        {
            switch (optionIndex)
            {
                default:
                case 0: // StreamingAssets
                    return Path.Combine(Application.streamingAssetsPath, "AssetBundles");
                case 2:
                    string customPath = EditorUtility.OpenFolderPanel("Select Build Path", Application.dataPath, "");
                    return string.IsNullOrEmpty(customPath) ? null : customPath;
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

            if(choice == 1)
                return;

            string outputPath = GetBuildPath(choice);
            outputPath += $"/{PlayerSettings.applicationIdentifier}";
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("AssetBundles built at: " + outputPath);
        }
    }
}
#endif

