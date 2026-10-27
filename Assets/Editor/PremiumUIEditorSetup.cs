#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class PremiumUIEditorSetup
{
    const string GeneratedFolder = "Assets/UI/Premium/Generated";
    const string ThemePath = "Assets/Resources/PremiumUITheme.asset";

    [MenuItem("Trash Cat/Premium UI/Generate Sprites And Theme")]
    public static void GeneratePremiumAssets()
    {
        Directory.CreateDirectory(GeneratedFolder);
        Directory.CreateDirectory("Assets/Resources");

        var panelSurface = SaveSprite("PanelSurface", 128, 128, PremiumUIColor.SurfaceWhite, PremiumUIColor.PrimaryBlue, 0.15f);
        var panelNavy = SaveSprite("PanelNavy", 128, 128, PremiumUIColor.DeepNavy, new Color(0.05f, 0.1f, 0.18f, 1f), 0.2f);
        var card = SaveSprite("CardElevated", 128, 128, Color.white, new Color(0.85f, 0.89f, 0.96f, 1f), 0.25f);
        var btnPrimary = SaveSprite("ButtonPrimary", 128, 64, PremiumUIColor.AccentOrange, new Color(0.85f, 0.45f, 0.05f, 1f), 0.15f);
        var btnSecondary = SaveSprite("ButtonSecondary", 128, 64, PremiumUIColor.PrimaryBlue, new Color(0.1f, 0.28f, 0.62f, 1f), 0.15f);
        var btnDisabled = SaveSprite("ButtonDisabled", 128, 64, new Color(0.78f, 0.82f, 0.88f, 1f), new Color(0.62f, 0.66f, 0.72f, 1f), 0.1f);
        var btnDanger = SaveSprite("ButtonDanger", 128, 64, new Color(0.82f, 0.22f, 0.24f, 1f), new Color(0.62f, 0.12f, 0.14f, 1f), 0.15f);
        var tabActive = SaveSprite("TabActive", 128, 48, PremiumUIColor.PrimaryBlue, new Color(0.08f, 0.22f, 0.52f, 1f), 0.12f);
        var tabInactive = SaveSprite("TabInactive", 128, 48, new Color(0.93f, 0.95f, 0.99f, 1f), new Color(0.82f, 0.86f, 0.92f, 1f), 0.12f);
        var progressBg = SaveSprite("ProgressBackground", 256, 24, new Color(0.92f, 0.94f, 0.97f, 1f), new Color(0.75f, 0.78f, 0.84f, 0.5f), 0.08f, 10);
        var progressFill = SaveSprite("ProgressFill", 256, 24, Ui20Palette.MintGreen, new Color(0.05f, 0.55f, 0.42f, 1f), 0.08f, 10);
        var overlay = SaveSprite("OverlayDim", 64, 64, new Color(0.04f, 0.08f, 0.14f, 0.85f), Color.clear, 0f);

        var theme = AssetDatabase.LoadAssetAtPath<PremiumUITheme>(ThemePath);
        if (theme == null)
        {
            theme = ScriptableObject.CreateInstance<PremiumUITheme>();
            AssetDatabase.CreateAsset(theme, ThemePath);
        }

        theme.panelSurface = panelSurface;
        theme.panelNavy = panelNavy;
        theme.cardElevated = card;
        theme.buttonPrimary = btnPrimary;
        theme.buttonSecondary = btnSecondary;
        theme.buttonDisabled = btnDisabled;
        theme.buttonDanger = btnDanger;
        theme.tabActive = tabActive;
        theme.tabInactive = tabInactive;
        theme.progressBackground = progressBg;
        theme.progressFill = progressFill;
        theme.overlayDim = overlay;
        theme.sliceBorder = 32;

        EditorUtility.SetDirty(theme);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Premium UI sprites and theme generated.");
    }

    static Sprite SaveSprite(string name, int width, int height, Color fill, Color border, float borderStrength, int radius = 18)
    {
        string path = $"{GeneratedFolder}/{name}.png";
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        int slice = Mathf.Min(radius, Mathf.Min(width, height) / 2);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inside = InsideRoundedRect(x, y, width, height, slice);
                bool edge = inside && NearEdge(x, y, width, height, slice, 2);
                Color c = !inside ? Color.clear : (edge && borderStrength > 0f ? Color.Lerp(fill, border, borderStrength) : fill);
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            float borderPx = slice;
            importer.spriteBorder = new Vector4(borderPx, borderPx, borderPx, borderPx);
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static bool InsideRoundedRect(int x, int y, int w, int h, int r)
    {
        if (x < r && y < r) return (new Vector2(x - r, y - r)).magnitude <= r;
        if (x >= w - r && y < r) return (new Vector2(x - (w - r - 1), y - r)).magnitude <= r;
        if (x < r && y >= h - r) return (new Vector2(x - r, y - (h - r - 1))).magnitude <= r;
        if (x >= w - r && y >= h - r) return (new Vector2(x - (w - r - 1), y - (h - r - 1))).magnitude <= r;
        return x >= 0 && y >= 0 && x < w && y < h;
    }

    static bool NearEdge(int x, int y, int w, int h, int r, int thickness)
    {
        for (int i = 1; i <= thickness; i++)
        {
            if (!InsideRoundedRect(x - i, y, w, h, r)) return true;
            if (!InsideRoundedRect(x + i, y, w, h, r)) return true;
            if (!InsideRoundedRect(x, y - i, w, h, r)) return true;
            if (!InsideRoundedRect(x, y + i, w, h, r)) return true;
        }
        return false;
    }

    static void EnsureComponent<T>(GameObject go) where T : Component
    {
        if (go.GetComponent<T>() == null)
            go.AddComponent<T>();
    }

    static void EnsurePrefabComponent<T>(string prefabPath) where T : Component
    {
        var root = PrefabUtility.LoadPrefabContents(prefabPath);
        EnsureComponent<T>(root);
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    [MenuItem("Trash Cat/Premium UI/Modernize Store")]
    public static void ModernizePremiumStore()
    {
        GeneratePremiumAssets();
        EnsurePrefabComponent<PremiumShopItemStyle>("Assets/Prefabs/UI/ItemEntry.prefab");

        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Shop.unity");
        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
            EnsureComponent<PremiumShopScreenStyle>(canvas);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("Store premium UI applied.");
    }

    public static void ModernizePremiumStoreBatch() => ModernizePremiumStore();

    [MenuItem("Trash Cat/Premium UI/Modernize Missions")]
    public static void ModernizePremiumMissions()
    {
        GeneratePremiumAssets();
        EnsurePrefabComponent<PremiumMissionCardStyle>("Assets/Prefabs/UI/MissionEntry.prefab");
        EnsurePrefabComponent<PremiumMissionPopupStyle>("Assets/Prefabs/UI/MissionPopup.prefab");

        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        var popup = GameObject.Find("MissionPopup");
        if (popup != null)
            EnsureComponent<PremiumMissionPopupStyle>(popup);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("Missions premium UI applied.");
    }

    public static void ModernizePremiumMissionsBatch() => ModernizePremiumMissions();

    [MenuItem("Trash Cat/Premium UI/Modernize Settings")]
    public static void ModernizePremiumSettings()
    {
        GeneratePremiumAssets();
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        var settings = GameObject.Find("SettingPopup");
        if (settings != null)
            EnsureComponent<PremiumSettingsScreenStyle>(settings);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("Settings premium UI applied.");
    }

    public static void ModernizePremiumSettingsBatch() => ModernizePremiumSettings();

    [MenuItem("Trash Cat/Premium UI/Modernize Game Over")]
    public static void ModernizePremiumGameOver()
    {
        GeneratePremiumAssets();
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        var states = Object.FindObjectsByType<GameOverState>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var state in states)
        {
            if (state.canvas != null)
                EnsureComponent<PremiumGameOverScreenStyle>(state.canvas.gameObject);
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game Over premium UI applied.");
    }

    public static void ModernizePremiumGameOverBatch() => ModernizePremiumGameOver();
}
#endif
