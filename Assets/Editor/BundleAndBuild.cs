using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Builds Addressables player content for the active build target (required for Android device builds).
/// </summary>
public static class BundleAndBuild
{
    const string SettingsRelativePath = "aa/settings.json";

    [MenuItem("Trash Dash Debug/Build with Bundle/Build Addressables (Active Target)")]
    public static void BuildAddressablesForActiveTarget()
    {
        BuildAddressablesPlayerContentInternal(EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("Trash Dash Debug/Build with Bundle/Build Addressables (Android)")]
    public static void BuildAddressablesAndroid()
    {
        BuildAddressablesPlayerContentInternal(BuildTarget.Android);
    }

    /// <summary>Used by Unity batchmode: -executeMethod BundleAndBuild.BuildAddressablesAndroidBatch</summary>
    public static void BuildAddressablesAndroidBatch()
    {
        BuildAddressablesPlayerContentInternal(BuildTarget.Android);
    }

    static void BuildAddressablesPlayerContentInternal(BuildTarget target)
    {
        var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
        if (EditorUserBuildSettings.activeBuildTarget != target)
            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found. Open Window > Asset Management > Addressables > Groups once.");
            ExitBatchMode(1);
            return;
        }

        if (settings.ActivePlayerDataBuilder == null)
        {
            Debug.LogError("Addressables active player data builder is null.");
            ExitBatchMode(1);
            return;
        }

        Debug.Log("Building Addressables player content for " + target + " using " + settings.ActivePlayerDataBuilder.Name);

        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);

        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.LogError("Addressables build failed: " + result.Error);
            ExitBatchMode(1);
            return;
        }

        LogStreamingAssetsPresence(target);
        Debug.Log("Addressables build succeeded. Duration: " + result.Duration + "s");
    }

    static void ExitBatchMode(int code)
    {
        if (Application.isBatchMode)
            EditorApplication.Exit(code);
    }

    static void LogStreamingAssetsPresence(BuildTarget target)
    {
        var platformFolder = target.ToString();
        var librarySettingsPath = Path.Combine(Addressables.BuildPath, "settings.json");
        var libraryPlatformDir = Addressables.BuildPath;

        Debug.Log("Addressables player build path: " + Addressables.BuildPath);
        Debug.Log(
            "Library settings.json exists: "
            + File.Exists(librarySettingsPath)
            + " ("
            + librarySettingsPath
            + ")");
        Debug.Log(
            "Library platform dir exists: "
            + Directory.Exists(libraryPlatformDir)
            + " ("
            + libraryPlatformDir
            + ")");

        // AA 2.x keeps built content under Library until an Android/iOS player build injects it into the APK/IPA.
        var streamingSettings = Path.Combine(Application.streamingAssetsPath, SettingsRelativePath);
        Debug.Log(
            "Assets/StreamingAssets mirror (optional): "
            + File.Exists(streamingSettings)
            + " — player build adds Library/aa to StreamingAssets automatically.");
    }

    [MenuItem("Trash Dash Debug/Build with Bundle/Build and Run")]
    static void BuildAndRun()
    {
        BuildAddressablesForActiveTarget();
    }

    [MenuItem("Trash Dash Debug/Build with Bundle/Build")]
    static void Build()
    {
        BuildAddressablesForActiveTarget();
    }
}
