using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PremiumMissionPopupStyle : MonoBehaviour
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

        var background = transform.Find("MissionBackground")?.GetComponent<Image>();
        if (background != null)
            PremiumUIStyler.ApplySlicedPanel(background, theme.panelSurface, PremiumUIColor.SurfaceWhite);

        var title = GetComponentInChildren<Text>(true);
        foreach (var text in GetComponentsInChildren<Text>(true))
        {
            if (text.name.Contains("Title") || text.text.ToUpperInvariant().Contains("MISSION"))
                PremiumUIStyler.StyleTitleText(text, PremiumUIColor.DeepNavy, 34);
        }

        var scroll = GetComponentInChildren<ScrollRect>(true);
        if (scroll != null && scroll.content != null)
        {
            var layout = scroll.content.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
                layout = scroll.content.gameObject.AddComponent<VerticalLayoutGroup>();

            layout.spacing = 12f;
            layout.padding = new RectOffset(12, 12, 12, 12);
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
        }

        if (GetComponent<PremiumUIFadeIn>() == null)
            gameObject.AddComponent<PremiumUIFadeIn>();
    }
}
