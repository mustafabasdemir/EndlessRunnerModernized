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
        CleanRebuildMissionPopupPrefab();
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

    static Sprite AtlasSpriteByName(string spriteName)
    {
        var path = AssetDatabase.GUIDToAssetPath(UiAtlasGuid);
        foreach (var a in AssetDatabase.LoadAllAssetsAtPath(path))
        {
            if (a is Sprite s && s.name == spriteName)
                return s;
        }

        return null;
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
        rootLe.minHeight = 160;
        rootLe.preferredHeight = -1;
        rootLe.flexibleWidth = 1f;
        var rootFitter = root.GetComponent<ContentSizeFitter>() ?? root.AddComponent<ContentSizeFitter>();
        rootFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var vlg = root.GetComponent<VerticalLayoutGroup>() ?? root.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(18, 18, 16, 16);
        vlg.spacing = 10;
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        var desc = CreateText(root.transform, "DescText", "Mission description", 24, Ui20Palette.BodyText, TextAnchor.UpperLeft);
        desc.horizontalOverflow = HorizontalWrapMode.Wrap;
        desc.verticalOverflow = VerticalWrapMode.Truncate;
        var descLe = desc.gameObject.AddComponent<LayoutElement>();
        descLe.minHeight = 64;
        descLe.preferredHeight = -1;

        var progressSection = CreateRect(root.transform, "ProgressSection");
        PrepareVerticalLayoutChild(progressSection, 52f);
        var progressSectionLe = progressSection.gameObject.AddComponent<LayoutElement>();
        progressSectionLe.minHeight = 52;
        progressSectionLe.preferredHeight = 52;

        var progressVlg = progressSection.gameObject.AddComponent<VerticalLayoutGroup>();
        progressVlg.spacing = 8;
        progressVlg.childControlWidth = true;
        progressVlg.childControlHeight = true;
        progressVlg.childForceExpandWidth = true;
        progressVlg.childForceExpandHeight = false;

        var progressText = CreateText(progressSection, "ProgressText", "0 / 100", 18, Ui20Palette.DeepNavy, TextAnchor.MiddleLeft);
        PrepareVerticalLayoutChild(progressText.rectTransform, 22f);
        var progressTextLe = progressText.gameObject.AddComponent<LayoutElement>();
        progressTextLe.minHeight = 22;
        progressTextLe.preferredHeight = 22;

        var track = CreateRect(progressSection, "ProgressTrack");
        PrepareVerticalLayoutChild(track, 22f);
        var trackLe = track.gameObject.AddComponent<LayoutElement>();
        trackLe.minHeight = 22;
        trackLe.preferredHeight = 22;
        trackLe.flexibleWidth = 1f;
        var trackBg = track.gameObject.AddComponent<Image>();
        ApplySlice(trackBg, theme != null ? theme.progressBackground : null, new Color(0.92f, 0.94f, 0.97f, 1f));
        track.gameObject.AddComponent<RectMask2D>();

        var fill = CreateRect(track, "Fill");
        fill.pivot = new Vector2(0f, 0.5f);
        fill.anchorMin = Vector2.zero;
        fill.anchorMax = new Vector2(0.35f, 1f);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
        var fillImg = fill.gameObject.AddComponent<Image>();
        ApplySlice(fillImg, theme != null ? theme.progressFill : null, Ui20Palette.MintGreen);
        fillImg.raycastTarget = false;

        var progressBar = track.gameObject.AddComponent<Ui20HorizontalProgressBar>();
        SetPrivateField(progressBar, "m_Fill", fill);

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
        rewardIcon.sprite = AtlasSpriteByName("PremiumIcone") ?? AtlasSpriteByName("FishboneIcone");
        rewardIcon.preserveAspect = true;
        rewardIcon.color = Color.white;

        var rewardText = CreateText(rewardBadge, "RewardText", "100", 28, Ui20Palette.AccentOrange, TextAnchor.MiddleLeft);
        rewardText.fontStyle = FontStyle.Bold;

        var spacer = CreateRect(footer, "Spacer");
        var spacerLe = spacer.gameObject.AddComponent<LayoutElement>();
        spacerLe.flexibleWidth = 1f;

        var claimBtn = CreateButton(footer, "ClaimButton", "CLAIM REWARD", theme);
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
        entry.notCompletedColor = Color.white;
        entry.completedColor = new Color(0.96f, 0.98f, 1f, 1f);

        PrefabUtility.SaveAsPrefabAsset(root, MissionEntryPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    static void CleanRebuildMissionPopupPrefab()
    {
        var root = PrefabUtility.LoadPrefabContents(MissionPopupPath);
        var missionUi = root.GetComponent<MissionUI>();
        RemoveComponentIfExists<PremiumMissionPopupStyle>(root);
        RemoveComponentIfExists<PremiumUIFadeIn>(root);
        var motion = root.GetComponent<Ui20MissionModalMotion>() ?? root.AddComponent<Ui20MissionModalMotion>();
        if (root.GetComponent<CanvasGroup>() == null)
            root.AddComponent<CanvasGroup>();

        var theme = Theme;
        var backdrop = root.GetComponent<Image>();
        if (backdrop != null)
        {
            ApplySlice(backdrop, theme != null ? theme.overlayDim : null, Ui20Palette.Overlay);
            backdrop.raycastTarget = true;
        }

        var panel = FindDeep(root.transform, "MissionBackground");
        var scrollView = FindDeep(root.transform, "Scroll View");
        var content = FindDeep(root.transform, "Content");
        var closeButton = FindDeep(root.transform, "CloseButton");

        if (panel == null || scrollView == null || content == null)
        {
            Debug.LogError("MissionPopup is missing MissionBackground, Scroll View, or Content.");
            PrefabUtility.UnloadPrefabContents(root);
            return;
        }

        if (closeButton != null)
            closeButton.SetParent(root.transform, false);
        scrollView.SetParent(root.transform, false);

        for (int i = panel.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(panel.GetChild(i).gameObject);

        var panelImg = panel.GetComponent<Image>();
        ApplySlice(panelImg, theme != null ? theme.panelSurface : null, Ui20Palette.SurfaceWhite);
        panel.anchorMin = new Vector2(0.05f, 0.12f);
        panel.anchorMax = new Vector2(0.95f, 0.88f);
        panel.offsetMin = Vector2.zero;
        panel.offsetMax = Vector2.zero;
        SetPrivateField(motion, "m_Panel", panel);

        var headerBand = CreateRect(panel, "HeaderBand");
        headerBand.anchorMin = new Vector2(0f, 1f);
        headerBand.anchorMax = new Vector2(1f, 1f);
        headerBand.pivot = new Vector2(0.5f, 1f);
        headerBand.sizeDelta = new Vector2(0f, 80f);
        headerBand.anchoredPosition = Vector2.zero;
        var headerImg = headerBand.gameObject.AddComponent<Image>();
        ApplySlice(headerImg, theme != null ? theme.tabActive : null, Ui20Palette.PrimaryBlue);

        var title = CreateText(headerBand, "TitleText", "MISSIONS", 36, Color.white, TextAnchor.MiddleCenter);
        Stretch(title.rectTransform);
        title.rectTransform.offsetMin = new Vector2(16f, 0f);
        title.rectTransform.offsetMax = new Vector2(-56f, 0f);
        title.fontStyle = FontStyle.Bold;

        if (closeButton != null)
        {
            closeButton.SetParent(headerBand, false);
            closeButton.anchorMin = new Vector2(1f, 0.5f);
            closeButton.anchorMax = new Vector2(1f, 0.5f);
            closeButton.pivot = new Vector2(1f, 0.5f);
            closeButton.anchoredPosition = new Vector2(-10f, 0f);
            closeButton.sizeDelta = new Vector2(48f, 48f);
            StyleCloseButton(closeButton);
        }

        var missionsContainer = CreateRect(panel, "MissionsContainer");
        Stretch(missionsContainer);
        missionsContainer.offsetMin = new Vector2(10f, 10f);
        missionsContainer.offsetMax = new Vector2(-10f, -90f);

        scrollView.SetParent(missionsContainer, false);
        Stretch(scrollView);

        var viewport = FindDeep(scrollView, "Viewport");
        if (viewport != null)
        {
            var vpImg = viewport.GetComponent<Image>() ?? viewport.gameObject.AddComponent<Image>();
            vpImg.raycastTarget = false;
            vpImg.color = new Color(0.97f, 0.98f, 1f, 1f);
        }

        content.SetParent(viewport != null ? viewport : scrollView, false);
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(0f, 0f);

        var contentVlg = content.GetComponent<VerticalLayoutGroup>() ?? content.gameObject.AddComponent<VerticalLayoutGroup>();
        contentVlg.spacing = 12;
        contentVlg.padding = new RectOffset(4, 4, 4, 8);
        contentVlg.childControlWidth = true;
        contentVlg.childControlHeight = true;
        contentVlg.childForceExpandWidth = true;
        contentVlg.childForceExpandHeight = false;

        var fitter = content.GetComponent<ContentSizeFitter>() ?? content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        if (missionUi != null)
            missionUi.missionPlace = content;

        PrefabUtility.SaveAsPrefabAsset(root, MissionPopupPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;
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

    static void PrepareVerticalLayoutChild(RectTransform rt, float preferredHeight)
    {
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(0f, preferredHeight);
    }

    static void StyleCloseButton(RectTransform closeRt)
    {
        var img = closeRt.GetComponent<Image>();
        var closeSprite = AtlasSpriteByName("CloseButton");
        if (img != null && closeSprite != null)
        {
            img.sprite = closeSprite;
            img.type = Image.Type.Simple;
            img.color = Color.white;
            img.preserveAspect = true;
        }

        for (int i = closeRt.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(closeRt.GetChild(i).gameObject);

        if (closeRt.GetComponent<Ui20ButtonPressFeedback>() == null)
            closeRt.gameObject.AddComponent<Ui20ButtonPressFeedback>();
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
