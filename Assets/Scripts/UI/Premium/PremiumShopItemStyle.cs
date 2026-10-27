using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PremiumShopItemStyle : MonoBehaviour
{
    [SerializeField] Image m_CardBackground;
    [SerializeField] Image m_IconFrame;
    [SerializeField] Text m_NameText;
    [SerializeField] Text m_PriceText;
    [SerializeField] Text m_PremiumText;
    [SerializeField] Text m_CountText;
    [SerializeField] Button m_BuyButton;

    bool m_Initialized;

    void Awake()
    {
        EnsureReferences();
        ApplyCardChrome();
    }

    void EnsureReferences()
    {
        if (m_Initialized)
            return;

        var item = GetComponent<ShopItemListItem>();
        m_CardBackground = m_CardBackground != null ? m_CardBackground : GetComponent<Image>();
        if (item != null)
        {
            m_NameText = item.nameText;
            m_PriceText = item.pricetext;
            m_PremiumText = item.premiumText;
            m_CountText = item.countText;
            m_BuyButton = item.buyButton;
        }

        m_IconFrame = transform.Find("Icon")?.GetComponent<Image>();
        m_Initialized = true;
    }

    void ApplyCardChrome()
    {
        var theme = PremiumUIThemeProvider.Active;
        if (theme == null)
            return;

        if (m_CardBackground != null)
            PremiumUIStyler.ApplySlicedPanel(m_CardBackground, theme.cardElevated, PremiumUIColor.SurfaceWhite);

        if (m_IconFrame != null)
        {
            m_IconFrame.color = new Color(1f, 1f, 1f, 0.12f);
            if (theme.panelSurface != null)
            {
                m_IconFrame.sprite = theme.panelSurface;
                m_IconFrame.type = Image.Type.Sliced;
            }
        }

        PremiumUIStyler.StyleTitleText(m_NameText, PremiumUIColor.DeepNavy, 26);
        PremiumUIStyler.StyleBodyText(m_PriceText, PremiumUIColor.DeepNavy, 22, FontStyle.Bold);
        PremiumUIStyler.StyleBodyText(m_PremiumText, PremiumUIColor.AccentOrange, 20, FontStyle.Bold);
        PremiumUIStyler.StyleBodyText(m_CountText, PremiumUIColor.PrimaryBlue, 18);

        if (m_BuyButton != null)
            PremiumUIStyler.ApplyPrimaryButton(m_BuyButton, theme);
    }

    public void ApplyBuyState(ShopItemListItem item)
    {
        EnsureReferences();
        var theme = PremiumUIThemeProvider.Active;
        if (theme == null || item == null || item.buyButton == null)
            return;

        var button = item.buyButton;
        var image = button.targetGraphic as Image;
        if (image == null)
            return;

        if (button.interactable)
        {
            image.sprite = theme.buttonPrimary;
            image.color = Color.white;
        }
        else
        {
            image.sprite = theme.buttonDisabled != null ? theme.buttonDisabled : theme.buttonSecondary;
            image.color = new Color(1f, 1f, 1f, 0.85f);
        }

        var label = button.GetComponentInChildren<Text>(true);
        if (label != null)
            label.color = button.interactable ? PremiumUIColor.SurfaceWhite : new Color(0.45f, 0.5f, 0.58f, 1f);
    }
}
