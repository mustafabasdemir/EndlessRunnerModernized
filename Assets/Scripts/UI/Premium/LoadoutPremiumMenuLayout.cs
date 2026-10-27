using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsive layout and typography for the Main Menu / Loadout canvas only.
/// Preserves existing buttons, UnityEvents and LoadoutState behaviour.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class LoadoutPremiumMenuLayout : MonoBehaviour
{
    static readonly string[] BottomNavOrder =
    {
        "OpenLeaderboard",
        "StoreButton",
        "MissionButton",
        "SettingButton",
    };

    const float ReferenceWidth = 720f;
    const float HorizontalPadding = 24f;
    const float BottomNavHeight = 92f;
    const float NavVerticalInset = 12f;
    const float RunGapAboveNav = 14f;
    const float RunButtonHeight = 128f;
    const float MinNavItemWidth = 72f;
    const float MaxNavItemWidth = 108f;

    [SerializeField] RectTransform m_RunButton;
    [SerializeField] RectTransform m_CharZone;
    [SerializeField] Text m_CharacterNameLabel;
    [SerializeField] Text m_RunLabel;
    [SerializeField] List<RectTransform> m_BottomNavItems = new List<RectTransform>();

    Canvas m_Canvas;
    CanvasScaler m_Scaler;
    bool m_Initialized;

    void OnEnable()
    {
        EnsureReferences();
        ApplyLayout();
    }

    void OnRectTransformDimensionsChange()
    {
        if (isActiveAndEnabled)
            ApplyLayout();
    }

    void EnsureReferences()
    {
        if (m_Initialized)
            return;

        m_Canvas = GetComponent<Canvas>();
        m_Scaler = GetComponent<CanvasScaler>();

        var root = transform as RectTransform;
        if (m_RunButton == null)
            m_RunButton = root.Find("StartButton") as RectTransform;

        if (m_CharZone == null)
            m_CharZone = root.Find("CharZone") as RectTransform;

        if (m_CharacterNameLabel == null && m_CharZone != null)
        {
            var charName = m_CharZone.Find("CharName");
            if (charName != null)
                m_CharacterNameLabel = charName.GetComponent<Text>();
        }

        if (m_RunLabel == null && m_RunButton != null)
        {
            var textTf = m_RunButton.Find("Text");
            if (textTf != null)
                m_RunLabel = textTf.GetComponent<Text>();
        }

        if (m_BottomNavItems.Count == 0)
        {
            foreach (var name in BottomNavOrder)
            {
                var item = root.Find(name) as RectTransform;
                if (item != null)
                    m_BottomNavItems.Add(item);
            }
        }

        m_Initialized = true;
    }

    void ApplyLayout()
    {
        if (m_Canvas == null)
            return;

        float bottomInset = GetSafeBottomInsetCanvasUnits();
        float topInset = GetSafeTopInsetCanvasUnits();
        LayoutBottomNavigation(bottomInset);
        LayoutRunButton(bottomInset);
        LayoutCharacterHeader(topInset);
        ApplyTypography();
        ApplyRunButtonAccent();
    }

    float GetCanvasScaleFactor()
    {
        if (m_Canvas == null)
            return 1f;
        return Mathf.Max(0.01f, m_Canvas.scaleFactor);
    }

    float GetSafeBottomInsetCanvasUnits()
    {
        Rect safe = Screen.safeArea;
        float bottomPx = safe.y;
        return bottomPx / GetCanvasScaleFactor();
    }

    float GetSafeTopInsetCanvasUnits()
    {
        Rect safe = Screen.safeArea;
        float topPx = Screen.height - safe.yMax;
        return topPx / GetCanvasScaleFactor();
    }

    void LayoutBottomNavigation(float bottomInset)
    {
        if (m_BottomNavItems == null || m_BottomNavItems.Count == 0)
            return;

        float canvasWidth = ReferenceWidth;
        var root = transform as RectTransform;
        if (root != null && root.rect.width > 1f)
            canvasWidth = root.rect.width;

        float usable = canvasWidth - HorizontalPadding * 2f;
        int count = m_BottomNavItems.Count;
        float spacing = 10f;
        float itemWidth = (usable - spacing * (count - 1)) / count;
        itemWidth = Mathf.Clamp(itemWidth, MinNavItemWidth, MaxNavItemWidth);

        float totalWidth = itemWidth * count + spacing * (count - 1);
        float startX = -totalWidth * 0.5f + itemWidth * 0.5f;
        float y = bottomInset + NavVerticalInset + BottomNavHeight * 0.5f;

        for (int i = 0; i < count; i++)
        {
            var rt = m_BottomNavItems[i];
            if (rt == null)
                continue;

            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(itemWidth, BottomNavHeight);
            rt.anchoredPosition = new Vector2(startX + i * (itemWidth + spacing), y);

            StyleNavLabel(rt);
        }
    }

    static void StyleNavLabel(RectTransform navItem)
    {
        var label = navItem.GetComponentInChildren<Text>(true);
        if (label == null)
            return;

        label.color = PremiumUIColor.NavLabel;
        label.fontSize = 20;
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 14;
        label.resizeTextMaxSize = 22;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Truncate;

        var labelRt = label.rectTransform;
        labelRt.anchorMin = new Vector2(0f, 0f);
        labelRt.anchorMax = new Vector2(1f, 0f);
        labelRt.pivot = new Vector2(0.5f, 1f);
        labelRt.anchoredPosition = new Vector2(0f, -4f);
        labelRt.sizeDelta = new Vector2(0f, 36f);
    }

    void LayoutRunButton(float bottomInset)
    {
        if (m_RunButton == null)
            return;

        float navTop = bottomInset + NavVerticalInset + BottomNavHeight + RunGapAboveNav;
        float y = navTop + RunButtonHeight * 0.5f;

        m_RunButton.anchorMin = new Vector2(0f, 0f);
        m_RunButton.anchorMax = new Vector2(1f, 0f);
        m_RunButton.pivot = new Vector2(0.5f, 0.5f);
        m_RunButton.sizeDelta = new Vector2(-HorizontalPadding * 2f, RunButtonHeight);
        m_RunButton.anchoredPosition = new Vector2(0f, y);
    }

    void LayoutCharacterHeader(float topInset)
    {
        if (m_CharZone == null)
            return;

        const float headerHeight = 156f;
        m_CharZone.anchorMin = new Vector2(0.5f, 1f);
        m_CharZone.anchorMax = new Vector2(0.5f, 1f);
        m_CharZone.pivot = new Vector2(0.5f, 1f);
        m_CharZone.sizeDelta = new Vector2(ReferenceWidth, headerHeight);
        m_CharZone.anchoredPosition = new Vector2(0f, -topInset - 8f);
    }

    void ApplyTypography()
    {
        if (m_CharacterNameLabel != null)
        {
            m_CharacterNameLabel.color = PremiumUIColor.CharacterName;
            m_CharacterNameLabel.fontSize = 42;
            m_CharacterNameLabel.resizeTextForBestFit = true;
            m_CharacterNameLabel.resizeTextMinSize = 28;
            m_CharacterNameLabel.resizeTextMaxSize = 44;
        }

        if (m_RunLabel != null)
        {
            m_RunLabel.color = PremiumUIColor.RunLabel;
            m_RunLabel.fontSize = 46;
            m_RunLabel.fontStyle = FontStyle.Bold;
            m_RunLabel.resizeTextForBestFit = true;
            m_RunLabel.resizeTextMinSize = 32;
            m_RunLabel.resizeTextMaxSize = 52;
        }
    }

    void ApplyRunButtonAccent()
    {
        if (m_RunButton == null)
            return;

        var image = m_RunButton.GetComponent<Image>();
        if (image != null)
            image.color = PremiumUIColor.AccentOrange;

        var button = m_RunButton.GetComponent<Button>();
        if (button != null)
        {
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.92f, 0.82f, 1f);
            colors.pressedColor = new Color(0.85f, 0.5f, 0.08f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
        }
    }
}
