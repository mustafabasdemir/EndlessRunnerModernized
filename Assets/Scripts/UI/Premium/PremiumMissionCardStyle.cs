using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PremiumMissionCardStyle : MonoBehaviour
{
    [SerializeField] Image m_CardBackground;
    [SerializeField] Image m_ProgressFill;
    [SerializeField] RectTransform m_ProgressRoot;

    PremiumUITheme m_Theme;

    void Awake()
    {
        m_Theme = PremiumUIThemeProvider.Active;
        EnsureProgressBar();
        ApplyBaseChrome();
    }

    void EnsureProgressBar()
    {
        if (m_ProgressRoot != null)
            return;

        var entry = GetComponent<MissionEntry>();
        if (entry == null || entry.progressText == null)
            return;

        var progressHost = new GameObject("PremiumProgressBar", typeof(RectTransform), typeof(Image));
        m_ProgressRoot = progressHost.GetComponent<RectTransform>();
        m_ProgressRoot.SetParent(entry.progressText.transform.parent, false);
        m_ProgressRoot.SetSiblingIndex(entry.progressText.transform.GetSiblingIndex());

        m_ProgressRoot.anchorMin = new Vector2(0.05f, 0.12f);
        m_ProgressRoot.anchorMax = new Vector2(0.95f, 0.22f);
        m_ProgressRoot.offsetMin = Vector2.zero;
        m_ProgressRoot.offsetMax = Vector2.zero;

        var bg = m_ProgressRoot.GetComponent<Image>();
        if (m_Theme != null && m_Theme.progressBackground != null)
            PremiumUIStyler.ApplySlicedPanel(bg, m_Theme.progressBackground, new Color(1f, 1f, 1f, 0.25f));

        var fillGo = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        var fillRt = fillGo.GetComponent<RectTransform>();
        fillRt.SetParent(m_ProgressRoot, false);
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        fillRt.pivot = new Vector2(0f, 0.5f);

        m_ProgressFill = fillGo.GetComponent<Image>();
        if (m_Theme != null && m_Theme.progressFill != null)
            PremiumUIStyler.ApplySlicedPanel(m_ProgressFill, m_Theme.progressFill, PremiumUIColor.SuccessGreen);
    }

    void ApplyBaseChrome()
    {
        var entry = GetComponent<MissionEntry>();
        if (entry == null)
            return;

        m_CardBackground = entry.background;
        if (m_CardBackground != null && m_Theme != null)
            PremiumUIStyler.ApplySlicedPanel(m_CardBackground, m_Theme.cardElevated, PremiumUIColor.SurfaceWhite);

        PremiumUIStyler.StyleBodyText(entry.descText, PremiumUIColor.DeepNavy, 22);
        PremiumUIStyler.StyleBodyText(entry.rewardText, PremiumUIColor.AccentOrange, 24, FontStyle.Bold);
        PremiumUIStyler.StyleBodyText(entry.progressText, PremiumUIColor.DeepNavy, 20, FontStyle.Bold);

        if (entry.claimButton != null && m_Theme != null)
            PremiumUIStyler.ApplyPrimaryButton(entry.claimButton, m_Theme);
    }

    public void ApplyMissionState(MissionEntry entry, MissionBase mission)
    {
        if (entry == null || mission == null)
            return;

        if (mission.isComplete)
        {
            if (m_CardBackground != null)
                m_CardBackground.color = new Color(0.92f, 0.98f, 0.95f, 1f);
            PremiumUIStyler.StyleBodyText(entry.descText, PremiumUIColor.DeepNavy, 22);
            if (m_ProgressRoot != null)
                m_ProgressRoot.gameObject.SetActive(false);
        }
        else
        {
            if (m_CardBackground != null)
                m_CardBackground.color = PremiumUIColor.SurfaceWhite;

            entry.descText.color = new Color(0.35f, 0.42f, 0.52f, 1f);
            entry.progressText.color = PremiumUIColor.DeepNavy;

            if (m_ProgressRoot != null)
            {
                m_ProgressRoot.gameObject.SetActive(true);
                float ratio = mission.max > 0f ? Mathf.Clamp01(mission.progress / mission.max) : 0f;
                if (m_ProgressFill != null)
                    m_ProgressFill.rectTransform.localScale = new Vector3(ratio, 1f, 1f);
            }
        }
    }
}
