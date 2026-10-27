#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class MissionUI20Builder
{
    const string MissionEntryPath = "Assets/Prefabs/UI/MissionEntry.prefab";
    const string MissionPopupPath = "Assets/Prefabs/UI/MissionPopup.prefab";
    const string PreviewPath = "Assets/UI/Premium/Generated/MissionsUI20_Preview.png";
    const string UiAtlasGuid = "e1a536f74c7ef384a8d7132148ace0c8";
    const string FontGuid = "ab2cfde409d710b47b0b502877abb479";

    [MenuItem("Trash Cat/UI 2.0/Rebuild Missions UI")]
    public static void BuildMissionsUI20()
    {
        PremiumUIEditorSetup.GeneratePremiumAssets();
        RebuildMissionEntryPrefab();
        RestyleMissionPopupPrefab();
        AssetDatabase.SaveAssets();
        Debug.Log("Missions UI 2.0 rebuild complete.");
    }

    public static void BuildMissionsUI20Batch() => BuildMissionsUI20();

    public static void CaptureMissionsPreviewBatch()
    {
        BuildMissionsUI20();
        CapturePreview();
    }

    static PremiumUITheme Theme => AssetDatabase.LoadAssetAtPath<PremiumUITheme>("Assets/Resources/PremiumUITheme.asset");

    static Font UiFont => AssetDatabase.LoadAssetAtPath<Font>($"Assets/Fonts/LuckiestGuy.ttf") ??
                          AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(FontGuid));

    static Sprite CoinRewardSprite()
    {
        var path = AssetDatabase.GUIDToAssetPath(UiAtlasGuid);
        foreach (var a in AssetDatabase.LoadAllAssetsAtPath(path))
        {
            if (a is not Sprite s)
                continue;
            var n = s.name.ToLowerInvariant();
            if (n.Contains("coin") || n.Contains("fish") || n.Contains("premium_0"))
                return s;
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static void RebuildMissionEntryPrefab()
    {
        var root = PrefabUtility.LoadPrefabContents(MissionEntryPath);
        var entry = root.GetComponent<MissionEntry>();
        if (entry == null)
            entry = root.AddComponent<MissionEntry>();

        RemoveComponentIfExists<PremiumMissionCardStyle>(root);

        for (int i = root.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(root.transform.GetChild(i).gameObject);

        var theme = Theme;
        var cardImage = root.GetComponent<Image>() ?? root.AddComponent<Image>();
        ApplySlice(cardImage, theme != null ? theme.cardElevated : null, Ui20Palette.SurfaceWhite);

        var rootLe = root.GetComponent<LayoutElement>() ?? root.AddComponent<LayoutElement>();
        rootLe.minHeight = 196;
        rootLe.preferredHeight = 196;
        rootLe.flexibleWidth = 1f;

        var vlg = root.GetComponent<VerticalLayoutGroup>() ?? root.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(18, 18, 16, 16);
        vlg.spacing = 10;
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        var desc = CreateText(root.transform, "DescText", "Mission description", 26, Ui20Palette.BodyText, TextAnchor.UpperLeft);
        desc.horizontalOverflow = HorizontalWrapMode.Wrap;
        desc.verticalOverflow = VerticalWrapMode.Truncate;
        var descLe = desc.gameObject.AddComponent<LayoutElement>();
        descLe.minHeight = 72;
        descLe.preferredHeight = 72;
        descLe.flexibleHeight = 0;

        var progressSection = CreateRect(root.transform, "ProgressSection");
        var progressSectionLe = progressSection.gameObject.AddComponent<LayoutElement>();
        progressSectionLe.minHeight = 46;
        progressSectionLe.preferredHeight = 46;

        var progressVlg = progressSection.gameObject.AddComponent<VerticalLayoutGroup>();
        progressVlg.spacing = 6;
        progressVlg.childControlWidth = true;
        progressVlg.childControlHeight = true;
        progressVlg.childForceExpandHeight = false;

        var track = CreateRect(progressSection, "ProgressTrack");
        var trackLe = track.gameObject.AddComponent<LayoutElement>();
        trackLe.minHeight = 16;
        trackLe.preferredHeight = 16;
        var trackBg = track.gameObject.AddComponent<Image>();
        ApplySlice(trackBg, theme != null ? theme.progressBackground : null, new Color(1f, 1f, 1f, 0.45f));

        var fill = CreateRect(track, "Fill");
        fill.anchorMin = Vector2.zero;
        fill.anchorMax = new Vector2(0.35f, 1f);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
        var fillImg = fill.gameObject.AddComponent<Image>();
        ApplySlice(fillImg, theme != null ? theme.progressFill : null, Ui20Palette.MintGreen);

        var progressBar = track.gameObject.AddComponent<Ui20HorizontalProgressBar>();
        SetPrivateField(progressBar, "m_Fill", fill);

        var progressText = CreateText(progressSection, "ProgressText", "0 / 100", 20, Ui20Palette.DeepNavy, TextAnchor.MiddleLeft);
        var progressTextLe = progressText.gameObject.AddComponent<LayoutElement>();
        progressTextLe.minHeight = 24;

        var footer = CreateRect(root.transform, "Footer");
        var footerLe = footer.gameObject.AddComponent<LayoutElement>();
        footerLe.minHeight = 52;
        footerLe.preferredHeight = 52;
        var footerH = footer.gameObject.AddComponent<HorizontalLayoutGroup>();
        footerH.spacing = 12;
        footerH.childAlignment = TextAnchor.MiddleCenter;
        footerH.childControlWidth = true;
        footerH.childControlHeight = true;
        footerH.childForceExpandWidth = false;
        footerH.childForceExpandHeight = false;

        var rewardBadge = CreateRect(footer, "RewardBadge");
        var rewardLe = rewardBadge.gameObject.AddComponent<LayoutElement>();
        rewardLe.minWidth = 140;
        rewardLe.preferredWidth = 160;
        rewardLe.minHeight = 48;
        var rewardBg = rewardBadge.gameObject.AddComponent<Image>();
        ApplySlice(rewardBg, theme != null ? theme.panelSurface : null, new Color(1f, 1f, 1f, 0.65f));
        var rewardH = rewardBadge.gameObject.AddComponent<HorizontalLayoutGroup>();
        rewardH.padding = new RectOffset(10, 10, 6, 6);
        rewardH.spacing = 8;
        rewardH.childAlignment = TextAnchor.MiddleLeft;

        var rewardIconGo = CreateRect(rewardBadge, "RewardIcon");
        var rewardIconLe = rewardIconGo.gameObject.AddComponent<LayoutElement>();
        rewardIconLe.minWidth = 36;
        rewardIconLe.preferredWidth = 36;
        rewardIconLe.minHeight = 36;
        var rewardIcon = rewardIconGo.gameObject.AddComponent<Image>();
        rewardIcon.sprite = CoinRewardSprite();
        rewardIcon.preserveAspect = true;
        rewardIcon.color = Color.white;

        var rewardText = CreateText(rewardBadge, "RewardText", "100", 28, Ui20Palette.AccentOrange, TextAnchor.MiddleLeft);
        rewardText.fontStyle = FontStyle.Bold;

        var spacer = CreateRect(footer, "Spacer");
        var spacerLe = spacer.gameObject.AddComponent<LayoutElement>();
        spacerLe.flexibleWidth = 1f;

        var claimBtn = CreateButton(footer, "ClaimButton", "CLAIM", theme);
        var claimLe = claimBtn.GetComponent<LayoutElement>() ?? claimBtn.gameObject.AddComponent<LayoutElement>();
        claimLe.minWidth = 168;
        claimLe.preferredWidth = 168;
        claimLe.minHeight = 48;
        claimBtn.gameObject.AddComponent<Ui20ButtonPressFeedback>();

        entry.descText = desc;
        entry.rewardText = rewardText;
        entry.progressText = progressText;
        entry.claimButton = claimBtn;
        entry.background = cardImage;
        entry.progressFill = fillImg;
        entry.progressSection = progressSection.gameObject;
        entry.rewardIcon = rewardIcon;
        entry.notCompletedColor = Ui20Palette.SurfaceWhite;
        entry.completedColor = new Color(0.92f, 0.97f, 1f, 1f);

        PrefabUtility.SaveAsPrefabAsset(root, MissionEntryPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    static void RestyleMissionPopupPrefab()
    {
        var root = PrefabUtility.LoadPrefabContents(MissionPopupPath);
        RemoveComponentIfExists<PremiumMissionPopupStyle>(root);
        RemoveComponentIfExists<PremiumUIFadeIn>(root);

        var motion = root.GetComponent<Ui20MissionModalMotion>() ?? root.AddComponent<Ui20MissionModalMotion>();
        var group = root.GetComponent<CanvasGroup>() ?? root.AddComponent<CanvasGroup>();

        var backdrop = root.GetComponent<Image>();
        if (backdrop != null)
        {
            var theme = Theme;
            ApplySlice(backdrop, theme != null ? theme.overlayDim : null, Ui20Palette.Overlay);
            backdrop.raycastTarget = true;
        }

        var panel = FindDeep(root.transform, "MissionBackground");
        if (panel != null)
        {
            var panelImg = panel.GetComponent<Image>();
            if (panelImg != null)
                ApplySlice(panelImg, Theme != null ? Theme.panelSurface : null, Ui20Palette.SurfaceWhite);

            panel.anchorMin = new Vector2(0.06f, 0.14f);
            panel.anchorMax = new Vector2(0.94f, 0.86f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;

            SetPrivateField(motion, "m_Panel", panel);
        }

        foreach (var title in root.GetComponentsInChildren<Text>(true))
        {
            if (!title.text.ToUpperInvariant().Contains("MISSION"))
                continue;
            title.text = "MISSIONS";
            title.fontSize = 40;
            title.color = Ui20Palette.SurfaceWhite;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.horizontalOverflow = HorizontalWrapMode.Overflow;
            var shadow = title.GetComponent<Shadow>();
            if (shadow != null)
                Object.DestroyImmediate(shadow);
        }

        if (panel != null)
        {
            var headerBand = FindDeep(panel, "HeaderBand");
            if (headerBand == null)
            {
                headerBand = CreateRect(panel, "HeaderBand");
                headerBand.SetAsFirstSibling();
                headerBand.anchorMin = new Vector2(0f, 1f);
                headerBand.anchorMax = new Vector2(1f, 1f);
                headerBand.pivot = new Vector2(0.5f, 1f);
                headerBand.sizeDelta = new Vector2(0f, 96f);
                headerBand.anchoredPosition = Vector2.zero;
            }

            var headerImg = headerBand.GetComponent<Image>() ?? headerBand.gameObject.AddComponent<Image>();
            ApplySlice(headerImg, Theme != null ? Theme.tabActive : null, Ui20Palette.PrimaryBlue);

            var titleRt = FindDeep(panel, "Text");
            if (titleRt != null)
            {
                titleRt.SetParent(headerBand, false);
                titleRt.anchorMin = Vector2.zero;
                titleRt.anchorMax = Vector2.one;
                titleRt.offsetMin = new Vector2(72f, 0f);
                titleRt.offsetMax = new Vector2(-72f, 0f);
            }

            var closeRt = FindDeep(root.transform, "CloseButton");
            if (closeRt != null)
            {
                closeRt.SetParent(headerBand, false);
                closeRt.anchorMin = new Vector2(1f, 0.5f);
                closeRt.anchorMax = new Vector2(1f, 0.5f);
                closeRt.pivot = new Vector2(1f, 0.5f);
                closeRt.anchoredPosition = new Vector2(-12f, 0f);
                closeRt.sizeDelta = new Vector2(56f, 56f);
            }
        }

        var close = FindDeep(root.transform, "CloseButton");
        if (close != null)
        {
            var btn = close.GetComponent<Button>();
            if (btn != null && Theme != null)
                PremiumUIStyler.ApplySecondaryButton(btn, Theme);
            close.gameObject.AddComponent<Ui20ButtonPressFeedback>();
        }

        var scrollContent = FindDeep(root.transform, "Content");
        if (scrollContent != null)
        {
            var contentVlg = scrollContent.GetComponent<VerticalLayoutGroup>() ?? scrollContent.gameObject.AddComponent<VerticalLayoutGroup>();
            contentVlg.spacing = 14;
            contentVlg.padding = new RectOffset(14, 14, 8, 14);
            contentVlg.childControlWidth = true;
            contentVlg.childControlHeight = true;
            contentVlg.childForceExpandWidth = true;
            contentVlg.childForceExpandHeight = false;

            var fitter = scrollContent.GetComponent<ContentSizeFitter>() ?? scrollContent.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        PrefabUtility.SaveAsPrefabAsset(root, MissionPopupPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    static void CapturePreview()
    {
        var popup = PrefabUtility.LoadPrefabContents(MissionPopupPath);
        var entry = PrefabUtility.LoadPrefabContents(MissionEntryPath);

        var stage = EditorUtility.CreateGameObjectWithHideFlags("UI20PreviewStage", HideFlags.HideAndDontSave);
        try
        {
            var camGo = new GameObject("PreviewCamera", typeof(Camera));
            camGo.transform.SetParent(stage.transform, false);
            camGo.transform.position = new Vector3(0, 0, -10f);
            var cam = camGo.GetComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 1170f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Ui20Palette.DeepNavy;

            var canvasGo = new GameObject("PreviewCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(stage.transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 100f;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 2340);
            scaler.matchWidthOrHeight = 0.5f;

            var popupInst = Object.Instantiate(popup, canvasGo.transform);
            popupInst.SetActive(true);
            var popupRt = popupInst.transform as RectTransform;
            popupRt.sizeDelta = new Vector2(1080, 2340);
            popupRt.anchorMin = Vector2.zero;
            popupRt.anchorMax = Vector2.one;
            popupRt.offsetMin = Vector2.zero;
            popupRt.offsetMax = Vector2.zero;

            var missionUi = popupInst.GetComponent<MissionUI>();
            if (missionUi != null && missionUi.missionPlace != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    var card = Object.Instantiate(entry, missionUi.missionPlace, false);
                    var me = card.GetComponent<MissionEntry>();
                    if (me != null)
                    {
                        me.descText.text = i == 0 ? "Run 2000m in a single run" : "Collect 50 fish bones";
                        me.progressText.text = i == 0 ? "850 / 2000" : "32 / 50";
                        me.rewardText.text = i == 0 ? "250" : "100";
                        me.progressFill.rectTransform.anchorMax = new Vector2(i == 0 ? 0.42f : 0.64f, 1f);
                        me.claimButton.gameObject.SetActive(i == 1);
                        me.progressSection.SetActive(i == 0);
                    }
                }
            }

            Canvas.ForceUpdateCanvases();

            int width = 1080;
            int height = 2340;
            cam.orthographicSize = height * 0.5f;

            var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            System.IO.Directory.CreateDirectory("Assets/UI/Premium/Generated");
            System.IO.File.WriteAllBytes(PreviewPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            Object.DestroyImmediate(rt);

            AssetDatabase.ImportAsset(PreviewPath);
            Debug.Log($"Missions preview saved to {PreviewPath}");
        }
        finally
        {
            Object.DestroyImmediate(stage);
            PrefabUtility.UnloadPrefabContents(popup);
            PrefabUtility.UnloadPrefabContents(entry);
        }
    }

    static RectTransform CreateRect(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.layer = 5;
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.localScale = Vector3.one;
        return rt;
    }

    static Text CreateText(Transform parent, string name, string text, int size, Color color, TextAnchor align)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.layer = 5;
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.font = UiFont;
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.supportRichText = false;
        t.raycastTarget = false;
        return t;
    }

    static Button CreateButton(Transform parent, string name, string label, PremiumUITheme theme)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.layer = 5;
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        ApplySlice(img, theme != null ? theme.buttonPrimary : null, Color.white);
        var btn = go.GetComponent<Button>();
        PremiumUIStyler.ApplyPrimaryButton(btn, theme);
        var labelText = CreateText(go.transform, "Text", label, 22, Ui20Palette.SurfaceWhite, TextAnchor.MiddleCenter);
        var labelRt = labelText.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;
        return btn;
    }

    static void ApplySlice(Image image, Sprite sprite, Color color)
    {
        if (image == null)
            return;
        if (sprite != null)
        {
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
        }
        image.color = color;
    }

    static RectTransform FindDeep(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name)
                return t as RectTransform;
        }
        return null;
    }

    static T FindDeepComponent<T>(Transform root, string name) where T : Component
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name)
                return t.GetComponent<T>();
        }
        return null;
    }

    static void RemoveComponentIfExists<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c != null)
            Object.DestroyImmediate(c);
    }

    static void SetPrivateField(Object obj, string field, Object value)
    {
        var so = new SerializedObject(obj);
        so.FindProperty(field).objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
#endif
