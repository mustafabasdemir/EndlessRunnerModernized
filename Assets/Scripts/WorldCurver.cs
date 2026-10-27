using UnityEngine;

[ExecuteInEditMode]
public class WorldCurver : MonoBehaviour
{
	[Range(-0.1f, 0.1f)]
	public float curveStrength = 0.01f;

    int m_CurveStrengthID;
    float m_AppliedCurveStrength = float.NaN;

    private void OnEnable()
    {
        m_CurveStrengthID = Shader.PropertyToID("_CurveStrength");
        ApplyCurveStrengthIfChanged();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_CurveStrengthID == 0)
            m_CurveStrengthID = Shader.PropertyToID("_CurveStrength");
        m_AppliedCurveStrength = float.NaN;
        ApplyCurveStrengthIfChanged();
    }
#endif

	void Update()
	{
		ApplyCurveStrengthIfChanged();
	}

    void ApplyCurveStrengthIfChanged()
    {
        if (m_CurveStrengthID == 0)
            m_CurveStrengthID = Shader.PropertyToID("_CurveStrength");

        if (Mathf.Approximately(m_AppliedCurveStrength, curveStrength))
            return;

        m_AppliedCurveStrength = curveStrength;
        Shader.SetGlobalFloat(m_CurveStrengthID, curveStrength);
    }
}
