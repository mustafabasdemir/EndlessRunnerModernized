using UnityEngine;

public static class PremiumUIThemeProvider
{
    static PremiumUITheme s_Active;

    public static PremiumUITheme Active
    {
        get
        {
            if (s_Active != null)
                return s_Active;

            s_Active = Resources.Load<PremiumUITheme>("PremiumUITheme");
            return s_Active;
        }
    }

    public static void SetActive(PremiumUITheme theme)
    {
        s_Active = theme;
    }
}
