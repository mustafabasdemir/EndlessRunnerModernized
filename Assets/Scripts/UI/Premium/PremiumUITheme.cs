using UnityEngine;

[CreateAssetMenu(fileName = "PremiumUITheme", menuName = "Trash Cat/Premium UI Theme")]
public class PremiumUITheme : ScriptableObject
{
    [Header("Panels")]
    public Sprite panelSurface;
    public Sprite panelNavy;
    public Sprite cardElevated;
    public Sprite overlayDim;

    [Header("Buttons")]
    public Sprite buttonPrimary;
    public Sprite buttonSecondary;
    public Sprite buttonDisabled;
    public Sprite buttonDanger;

    [Header("Tabs")]
    public Sprite tabActive;
    public Sprite tabInactive;

    [Header("Progress")]
    public Sprite progressBackground;
    public Sprite progressFill;

    public int sliceBorder = 32;
}
