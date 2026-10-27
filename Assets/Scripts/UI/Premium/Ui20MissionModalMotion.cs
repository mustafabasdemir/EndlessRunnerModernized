using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))]
public class Ui20MissionModalMotion : MonoBehaviour
{
    [SerializeField] RectTransform m_Panel;
    [SerializeField] float m_Duration = 0.24f;

    CanvasGroup m_Group;

    void Awake()
    {
        m_Group = GetComponent<CanvasGroup>();
        if (m_Panel == null && transform.childCount > 0)
            m_Panel = transform.GetChild(0) as RectTransform;
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(PlayOpen());
    }

    IEnumerator PlayOpen()
    {
        m_Group.alpha = 0f;
        Vector3 startScale = Vector3.one * 0.94f;
        Vector3 endScale = Vector3.one;
        if (m_Panel != null)
            m_Panel.localScale = startScale;

        float t = 0f;
        while (t < m_Duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / m_Duration);
            m_Group.alpha = p;
            if (m_Panel != null)
                m_Panel.localScale = Vector3.LerpUnclamped(startScale, endScale, p);
            yield return null;
        }

        m_Group.alpha = 1f;
        if (m_Panel != null)
            m_Panel.localScale = endScale;
    }
}
