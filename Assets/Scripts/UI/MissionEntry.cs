using UnityEngine;
using UnityEngine.UI;

public class MissionEntry : MonoBehaviour
{
    public Text descText;
    public Text rewardText;
    public Button claimButton;
    public Text progressText;
    public Image background;

    public Image progressFill;
    public GameObject progressSection;
    public Image rewardIcon;

    public Color notCompletedColor;
    public Color completedColor;

    Ui20HorizontalProgressBar m_ProgressBar;

    void Awake()
    {
        m_ProgressBar = GetComponentInChildren<Ui20HorizontalProgressBar>(true);
    }

    public void FillWithMission(MissionBase m, MissionUI owner)
    {
        descText.text = m.GetMissionDesc().ToUpperInvariant();
        rewardText.text = "+" + m.reward;

        claimButton.onClick.RemoveAllListeners();

        float ratio = m.max > 0f ? m.progress / m.max : 0f;
        if (m_ProgressBar != null)
            m_ProgressBar.SetNormalized(ratio);
        else if (progressFill != null)
        {
            var rt = progressFill.rectTransform;
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(Mathf.Clamp01(ratio), 1f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        if (m.isComplete)
        {
            claimButton.gameObject.SetActive(true);
            SetProgressVisible(false);

            background.color = completedColor;
            descText.color = Ui20Palette.BodyText;
            rewardText.color = Ui20Palette.AccentOrange;
            progressText.color = Ui20Palette.MutedText;

            claimButton.onClick.AddListener(delegate { owner.Claim(m); });
        }
        else
        {
            claimButton.gameObject.SetActive(false);
            SetProgressVisible(true);

            background.color = notCompletedColor;
            descText.color = Ui20Palette.BodyText;
            rewardText.color = Ui20Palette.AccentOrange;
            progressText.color = Ui20Palette.DeepNavy;

            progressText.text = FormatProgressLine(m);
        }

        if (rewardIcon != null)
            rewardIcon.color = Color.white;
    }

    void SetProgressVisible(bool visible)
    {
        if (progressSection != null)
            progressSection.SetActive(visible);
        else if (progressText != null)
            progressText.gameObject.SetActive(visible);
    }

    static string FormatProgressLine(MissionBase m)
    {
        string desc = m.GetMissionDesc().ToLowerInvariant();
        int current = (int)m.progress;
        int target = (int)m.max;
        if (desc.Contains("single run") || desc.StartsWith("slide for"))
            return current + " / " + target + " m";
        return current + " / " + target;
    }
}
