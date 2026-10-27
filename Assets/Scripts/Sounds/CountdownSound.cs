using UnityEngine;

public class CountdownSound : MonoBehaviour
{
	protected AudioSource m_Source;
	protected float m_TimeToDisable;

    protected const float k_StartDelay = 0.5f;
	
	void OnEnable()
	{
		m_Source = GetComponent<AudioSource>();
		if (m_Source == null || m_Source.clip == null)
			return;

		m_TimeToDisable = m_Source.clip.length;
        AudioPlayUtility.PlayDelayed(m_Source, k_StartDelay);
	}

	void Update()
	{
		m_TimeToDisable -= Time.deltaTime;

		if (m_TimeToDisable < 0)
			gameObject.SetActive(false);
	}
}
