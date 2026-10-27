using UnityEngine;
using UnityEngine.EventSystems;

public class Ui20ButtonPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform m_Target;
    [SerializeField] float m_PressedScale = 0.96f;

    Vector3 m_DefaultScale = Vector3.one;

    void Awake()
    {
        if (m_Target == null)
            m_Target = transform as RectTransform;
        if (m_Target != null)
            m_DefaultScale = m_Target.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_Target != null)
            m_Target.localScale = m_DefaultScale * m_PressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_Target != null)
            m_Target.localScale = m_DefaultScale;
    }
}
