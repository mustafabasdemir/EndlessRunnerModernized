using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PremiumGameOverScreenStyle : MonoBehaviour
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

        var backdrop = GetComponent<Image>();
        if (backdrop != null && theme.overlayDim != null)
            PremiumUIStyler.ApplySlicedPanel(backdrop, theme.overlayDim, new Color(0.05f, 0.09f, 0.16f, 0.88f));

        foreach (var text in GetComponentsInChildren<Text>(true))
        {
            if (text.text.ToUpperInvariant().Contains("GAME OVER"))
            {
                PremiumUIStyler.StyleTitleText(text, PremiumUIColor.AccentOrange, 44);
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 28;
                text.resizeTextMaxSize = 44;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
            }
            else if (text.name.ToLowerInvariant().Contains("score") || text.name.ToLowerInvariant().Contains("number"))
                PremiumUIStyler.StyleTitleText(text, PremiumUIColor.SurfaceWhite, 28);
            else
                PremiumUIStyler.StyleBodyText(text, PremiumUIColor.SurfaceWhite, 20);
        }

        StylePrimaryAction("RunButton", theme);
        StylePrimaryAction("RunAgain", theme);
        StyleSecondaryAction("Loadout", theme);
        StyleSecondaryAction("Main", theme);

        var leaderboardPanel = transform.Find("LeaderboardPanel") ?? transform.Find("MiniLeaderboard");
        if (leaderboardPanel != null)
        {
            var img = leaderboardPanel.GetComponent<Image>();
            if (img != null && theme.cardElevated != null)
                PremiumUIStyler.ApplySlicedPanel(img, theme.cardElevated, PremiumUIColor.SurfaceWhite);
        }

        if (GetComponent<PremiumUIFadeIn>() == null)
            gameObject.AddComponent<PremiumUIFadeIn>();
    }

    void StylePrimaryAction(string nameFragment, PremiumUITheme theme)
    {
        var t = FindDeep(nameFragment);
        if (t == null)
            return;

        var button = t.GetComponent<Button>();
        if (button != null)
        {
            PremiumUIStyler.ApplyPrimaryButton(button, theme);
            var rt = t as RectTransform;
            if (rt != null)
            {
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, Mathf.Min(rt.sizeDelta.y, 120f));
            }
        }
    }

    void StyleSecondaryAction(string nameFragment, PremiumUITheme theme)
    {
        var t = FindDeep(nameFragment);
        if (t == null)
            return;

        var button = t.GetComponent<Button>();
        if (button != null)
            PremiumUIStyler.ApplySecondaryButton(button, theme);
    }

    Transform FindDeep(string nameFragment)
    {
        foreach (var tr in GetComponentsInChildren<Transform>(true))
        {
            if (tr.name.IndexOf(nameFragment, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return tr;
        }

        return null;
    }
}
