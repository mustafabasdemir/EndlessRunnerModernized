using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PremiumSettingsScreenStyle : MonoBehaviour
{
    void OnEnable()
    {
        Apply();
    }

    void Apply()
    {
        var theme = PremiumUIThemeProvider.Active;
        if (theme == null)
            return;

        var popup = GetComponent<SettingPopup>();
        var panel = GetComponent<Image>();
        if (panel != null)
            PremiumUIStyler.ApplySlicedPanel(panel, theme.panelSurface, PremiumUIColor.SurfaceWhite);

        foreach (var text in GetComponentsInChildren<Text>(true))
        {
            if (text.name.ToLowerInvariant().Contains("delete"))
                PremiumUIStyler.StyleBodyText(text, new Color(0.75f, 0.18f, 0.2f, 1f), 20, FontStyle.Bold);
            else if (text.name.ToLowerInvariant().Contains("title") || text.text.ToUpperInvariant().Contains("SETTING"))
                PremiumUIStyler.StyleTitleText(text, PremiumUIColor.DeepNavy, 36);
            else
                PremiumUIStyler.StyleBodyText(text, PremiumUIColor.DeepNavy, 22);
        }

        if (popup != null)
        {
            PremiumUIStyler.StyleSlider(popup.masterSlider, theme);
            PremiumUIStyler.StyleSlider(popup.musicSlider, theme);
            PremiumUIStyler.StyleSlider(popup.masterSFXSlider, theme);
        }

        var deleteButton = FindDeepChild(transform, "DeleteButton") ?? FindDeepChild(transform, "Delete Data");
        if (deleteButton != null)
        {
            var btn = deleteButton.GetComponent<Button>();
            if (btn != null)
                PremiumUIStyler.ApplyDangerButton(btn, theme);
        }

        var aboutButton = FindDeepChild(transform, "AboutButton") ?? FindDeepChild(transform, "About");
        if (aboutButton != null)
        {
            var btn = aboutButton.GetComponent<Button>();
            if (btn != null)
                PremiumUIStyler.ApplySecondaryButton(btn, theme);
        }

        if (GetComponent<PremiumUIFadeIn>() == null)
            gameObject.AddComponent<PremiumUIFadeIn>();
    }

    static Transform FindDeepChild(Transform root, string nameFragment)
    {
        foreach (var text in root.GetComponentsInChildren<Transform>(true))
        {
            if (text.name.IndexOf(nameFragment, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return text;
        }

        return null;
    }
}
