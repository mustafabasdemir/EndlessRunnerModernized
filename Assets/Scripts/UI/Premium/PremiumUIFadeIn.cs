using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))]
public class PremiumUIFadeIn : MonoBehaviour
{
    [SerializeField] float m_Duration = 0.22f;

    CanvasGroup m_Group;

    void Awake()
    {
        m_Group = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        if (m_Group == null)
            m_Group = gameObject.AddComponent<CanvasGroup>();

        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        m_Group.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < m_Duration)
        {
            elapsed += Time.unscaledDeltaTime;
            m_Group.alpha = Mathf.Clamp01(elapsed / m_Duration);
            yield return null;
        }

        m_Group.alpha = 1f;
    }
}
