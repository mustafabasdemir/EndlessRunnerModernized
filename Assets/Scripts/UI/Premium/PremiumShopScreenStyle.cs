using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PremiumShopScreenStyle : MonoBehaviour
{
    static readonly string[] TabNames = { "Character", "Themes", "Accesories", "Items" };

    [SerializeField] Image m_Backdrop;
    [SerializeField] Image m_HeaderBar;
    [SerializeField] Text m_CoinCounter;
    [SerializeField] Text m_PremiumCounter;

    void Start()
    {
        Apply();
    }

    void Apply()
    {
        var theme = PremiumUIThemeProvider.Active;
        if (theme == null)
            return;

        if (m_Backdrop == null)
            m_Backdrop = transform.Find("Container")?.GetComponentInChildren<Image>(true);

        if (m_Backdrop != null)
            PremiumUIStyler.ApplySlicedPanel(m_Backdrop, theme.panelNavy, new Color(0.08f, 0.14f, 0.24f, 0.98f));

        var header = transform.Find("Container/Header");
        if (header != null)
        {
            m_HeaderBar = header.GetComponent<Image>();
            if (m_HeaderBar != null)
                PremiumUIStyler.ApplySlicedPanel(m_HeaderBar, theme.panelSurface, PremiumUIColor.PrimaryBlue);
        }

        var shop = GetComponent<ShopUI>();
        if (shop != null)
        {
            PremiumUIStyler.StyleTitleText(shop.coinCounter, PremiumUIColor.SurfaceWhite, 24);
            PremiumUIStyler.StyleTitleText(shop.premiumCounter, PremiumUIColor.AccentOrange, 24);
        }

        StyleTabs(theme);
        EnsureFadeIn();
    }

    void StyleTabs(PremiumUITheme theme)
    {
        foreach (var tabName in TabNames)
        {
            var tab = FindDeepChild(transform, tabName);
            if (tab == null)
                continue;

            var button = tab.GetComponent<Button>();
            if (button == null)
                continue;

            PremiumUIStyler.ApplyTabButton(button, theme, false);
        }
    }

    static Transform FindDeepChild(Transform root, string name)
    {
        if (root.name == name)
            return root;

        for (int i = 0; i < root.childCount; i++)
        {
            var found = FindDeepChild(root.GetChild(i), name);
            if (found != null)
                return found;
        }

        return null;
    }

    void EnsureFadeIn()
    {
        if (GetComponent<PremiumUIFadeIn>() == null)
            gameObject.AddComponent<PremiumUIFadeIn>();
    }
}
