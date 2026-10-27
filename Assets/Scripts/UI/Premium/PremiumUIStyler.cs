using UnityEngine;
using UnityEngine.UI;

public static class PremiumUIStyler
{
    public static void ApplySlicedPanel(Image image, Sprite sprite, Color tint)
    {
        if (image == null || sprite == null)
            return;

        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.color = tint;
        image.pixelsPerUnitMultiplier = 1f;
    }

    public static void ApplyPrimaryButton(Button button, PremiumUITheme theme)
    {
        if (button == null || theme == null)
            return;

        var image = button.targetGraphic as Image;
        if (image != null && theme.buttonPrimary != null)
        {
            image.sprite = theme.buttonPrimary;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
        }

        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.94f, 0.86f, 1f);
        colors.pressedColor = new Color(0.88f, 0.52f, 0.08f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.75f, 0.78f, 0.85f, 0.65f);
        button.colors = colors;

        if (theme.buttonDisabled != null)
        {
            var state = button.spriteState;
            state.disabledSprite = theme.buttonDisabled;
            button.spriteState = state;
        }

        StyleButtonLabel(button, PremiumUIColor.SurfaceWhite, 22, FontStyle.Bold);
    }

    public static void ApplySecondaryButton(Button button, PremiumUITheme theme)
    {
        if (button == null || theme == null)
            return;

        var image = button.targetGraphic as Image;
        if (image != null && theme.buttonSecondary != null)
        {
            image.sprite = theme.buttonSecondary;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
        }

        StyleButtonLabel(button, PremiumUIColor.PrimaryBlue, 20, FontStyle.Bold);
    }

    public static void ApplyDangerButton(Button button, PremiumUITheme theme)
    {
        if (button == null || theme == null)
            return;

        var image = button.targetGraphic as Image;
        if (image != null && theme.buttonDanger != null)
        {
            image.sprite = theme.buttonDanger;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
        }

        StyleButtonLabel(button, PremiumUIColor.SurfaceWhite, 20, FontStyle.Bold);
    }

    public static void ApplyTabButton(Button button, PremiumUITheme theme, bool active)
    {
        if (button == null || theme == null)
            return;

        var image = button.targetGraphic as Image;
        if (image == null)
            return;

        image.sprite = active ? theme.tabActive : theme.tabInactive;
        image.type = Image.Type.Sliced;
        image.color = Color.white;
        StyleButtonLabel(button, active ? PremiumUIColor.SurfaceWhite : PremiumUIColor.DeepNavy, 18, FontStyle.Bold);
    }

    public static void StyleBodyText(Text text, Color color, int size, FontStyle style = FontStyle.Normal)
    {
        if (text == null)
            return;

        text.color = color;
        text.fontSize = size;
        text.fontStyle = style;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.supportRichText = false;
    }

    public static void StyleTitleText(Text text, Color color, int size)
    {
        if (text == null)
            return;

        text.color = color;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.alignment = TextAnchor.MiddleCenter;
    }

    public static void StyleSlider(Slider slider, PremiumUITheme theme)
    {
        if (slider == null || theme == null)
            return;

        var bg = slider.transform.Find("Background")?.GetComponent<Image>();
        var fill = slider.fillRect?.GetComponent<Image>();
        var handle = slider.handleRect?.GetComponent<Image>();

        if (bg != null && theme.progressBackground != null)
            ApplySlicedPanel(bg, theme.progressBackground, new Color(1f, 1f, 1f, 0.35f));
        if (fill != null && theme.progressFill != null)
            ApplySlicedPanel(fill, theme.progressFill, PremiumUIColor.PrimaryBlue);
        if (handle != null && theme.buttonPrimary != null)
        {
            handle.sprite = theme.buttonPrimary;
            handle.type = Image.Type.Sliced;
            handle.color = PremiumUIColor.AccentOrange;
        }
    }

    static void StyleButtonLabel(Button button, Color color, int size, FontStyle style)
    {
        var label = button.GetComponentInChildren<Text>(true);
        if (label == null)
            return;

        label.color = color;
        label.fontSize = size;
        label.fontStyle = style;
        label.alignment = TextAnchor.MiddleCenter;
    }
}
