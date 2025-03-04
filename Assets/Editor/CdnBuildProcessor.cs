using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

namespace CustomBuild.Web.CdnHelper
{
public class CdnBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    // Static field to assign a CdnLoaderData asset in the Editor
    [SerializeField]
    [Tooltip("The CdnLoaderData asset to use for injecting CDN scripts into the WebGL build.")]
    private static CdnLoaderData buildCdnData;
    public static void SetBuildCdnData(CdnLoaderData data) => buildCdnData = data;
    public static CdnLoaderData BuildCdnData => buildCdnData;
    
    [MenuItem("WebUtils/Open Configure CDN Build Window")]
    public static void OpenConfigureCdnBuildWindow()
    {
        if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            bool switchPlatform = EditorUtility.DisplayDialog(
                    "Build Target Warning",
                    "The current build target is not WebGL. CDN injection only works in WebGL builds.\n\nWould you like to switch to WebGL now?",
                    "Switch to WebGL",
                    "Cancel"
                );
            if (switchPlatform)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
                Debug.Log("Switched build target to WebGL.");
            }
            else
            {
                Debug.LogWarning("CDN configuration cancelled. Build target must be WebGL for CDN injection to work.");
                return; // Exit if user cancels
            }
        }
        
        CdnBuildConfigWindow.ShowWindow();
    }

    // Custom attribute to expose this field in the Inspector (requires an Editor script)
    [MenuItem("WebUtils/Configure CDN Build Data")]
    private static void ConfigureCdnBuildData()
    {
        if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            bool switchPlatform = EditorUtility.DisplayDialog(
                    "Build Target Warning",
                    "The current build target is not WebGL. CDN injection only works in WebGL builds.\n\nWould you like to switch to WebGL now?",
                    "Switch to WebGL",
                    "Cancel"
                );
            if (switchPlatform)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
                Debug.Log("Switched build target to WebGL.");
            }
            else
            {
                Debug.LogWarning("CDN configuration cancelled. Build target must be WebGL for CDN injection to work.");
                return; // Exit if user cancels
            }
        }
        
        // Open a simple selection window (or use a more complex Editor window if preferred)
        string projectPath = Path.GetFullPath(Application.dataPath); // Full path to Assets folder
        string absolutePath = EditorUtility.OpenFilePanelWithFilters("Select CDN Build Data", "Assets", new[] { "ScriptableObject", "asset" });
        if (string.IsNullOrEmpty(absolutePath))
        {
            return; // Cancelled
        }

        // Convert absolute path to relative path
        string relativePath = Path.GetRelativePath(projectPath, absolutePath);
        if (!relativePath.StartsWith("Assets"))
        {
            relativePath = "Assets/" + relativePath;
        }

        CdnLoaderData selected = AssetDatabase.LoadAssetAtPath<CdnLoaderData>(relativePath);
        if(selected != null)
        {
            buildCdnData = selected;
            EditorPrefs.SetString("CdnBuildProcessor_BuildCdnData", AssetDatabase.GetAssetPath(buildCdnData));
            Debug.Log($"CDN Build Data set to: {AssetDatabase.GetAssetPath(buildCdnData)}");
            EditorGUIUtility.PingObject(selected);
        }
        else
        {
            Debug.LogError("Selected file is not a valid asset.");
        }
    }

    [InitializeOnLoadMethod]
    private static void LoadBuildCdnData()
    {
        // Load the saved asset path on Editor startup
        string assetPath = EditorPrefs.GetString("CdnBuildProcessor_BuildCdnData", "");
        if (!string.IsNullOrEmpty(assetPath))
        {
            buildCdnData = AssetDatabase.LoadAssetAtPath<CdnLoaderData>(assetPath);
            if (buildCdnData == null)
            {
                Debug.LogWarning($"Failed to load CdnLoaderData from saved path: {assetPath}. It may have been moved or deleted.");
            }
            else
            {
                Debug.Log($"Loaded CDN Build Data from: {assetPath} (Click here to locate)", buildCdnData);
            }
        }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
        {
            return; // Only process WebGL builds
        }

        string indexHtmlPath = Path.Combine(report.summary.outputPath, "index.html");
        if (!File.Exists(indexHtmlPath))
        {
            Debug.LogWarning($"index.html not found at {indexHtmlPath}. CDN injection skipped.");
            return;
        }

        if (buildCdnData == null)
        {
            Debug.LogWarning("No CdnLoaderData assigned for build-time injection. Configure via 'WebUtils/Configure CDN Build Data'. Skipping CDN injection.");
            return;
        }

        string htmlContent = File.ReadAllText(indexHtmlPath);
        string headEndTag = "</head>";
        int headEndIndex = htmlContent.IndexOf(headEndTag);

        if (headEndIndex == -1)
        {
            Debug.LogWarning("Could not find </head> tag in index.html. CDN injection skipped.");
            return;
        }

        string scriptTags = "";
        if (buildCdnData.allowLoadScript)
        {
            foreach (var url in buildCdnData.ScriptList)
            {
                scriptTags += $"<script src=\"{url}\"></script>\n";
                Debug.Log($"Added CDN script to build: {url}");
            }
        }

        if (buildCdnData.allowLoadModule)
        {
            foreach (var url in buildCdnData.ModuleList)
            {
                scriptTags += $"<script type=\"module\" src=\"{url}\"></script>\n";
                Debug.Log($"Added CDN module to build: {url}");
            }
        }

        if (!string.IsNullOrEmpty(scriptTags))
        {
            htmlContent = htmlContent.Insert(headEndIndex, scriptTags);
            File.WriteAllText(indexHtmlPath, htmlContent);
            Debug.Log("Successfully injected CDN scripts into index.html.");
        }
    }
}

public class CdnBuildConfigWindow : EditorWindow
{
    private CdnLoaderData selectedData;

    public static void ShowWindow()
    {
        GetWindow<CdnBuildConfigWindow>("CDN Build Config");
    }

    private void OnEnable()
    {
        selectedData = CdnBuildProcessor.BuildCdnData;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Current CDN Build Data:", EditorStyles.boldLabel);
        selectedData = EditorGUILayout.ObjectField("Asset", selectedData, typeof(CdnLoaderData), false) as CdnLoaderData;

        if (GUILayout.Button("Apply"))
        {
            if (selectedData != null)
            {
                CdnBuildProcessor.SetBuildCdnData(selectedData);
                string relativePath = AssetDatabase.GetAssetPath(selectedData);
                EditorPrefs.SetString("CdnBuildProcessor_BuildCdnData", relativePath);
                Debug.Log($"CDN Build Data set to: {relativePath} (Click here to locate)", selectedData);
                EditorGUIUtility.PingObject(selectedData); // Highlight in Project window
            }
            else
            {
                Debug.LogWarning("No CdnLoaderData selected.");
                CdnBuildProcessor.SetBuildCdnData(null);
                EditorPrefs.DeleteKey("CdnBuildProcessor_BuildCdnData");
            }
        }
    }
}
}