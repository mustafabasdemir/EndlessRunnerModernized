using UnityEngine;

/// <summary>Prefab-based horizontal fill bar (anchor stretch, not vertical scale hacks).</summary>
public class Ui20HorizontalProgressBar : MonoBehaviour
{
    [SerializeField] RectTransform m_Fill;

    public void SetNormalized(float value)
    {
        if (m_Fill == null)
            return;

        float t = Mathf.Clamp01(value);
        m_Fill.anchorMin = new Vector2(0f, 0f);
        m_Fill.anchorMax = new Vector2(t, 1f);
        m_Fill.offsetMin = Vector2.zero;
        m_Fill.offsetMax = Vector2.zero;
    }
}
