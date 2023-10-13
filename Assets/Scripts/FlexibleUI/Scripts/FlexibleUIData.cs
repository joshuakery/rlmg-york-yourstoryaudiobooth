using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CharacterSpacingOptions
{
    public float character = 0f;
    public float word = 0f;
    public float line = 0f;
    public float paragraph = 0f;
}

[System.Serializable]
public class TextType
{
    public float size = 36f;
    public TMP_FontAsset font;
    public FontWeight weight = FontWeight.Regular;
    public CharacterSpacingOptions spacingOptions;
}
[System.Serializable]
public class Typography
{
    public TextType title;
    public TextType subtitle;
    public TextType h1;
    public TextType h2;
    public TextType h3;
    public TextType h4;
    public TextType p;
    public TextType caption;
}

public enum ColorChoice
{
    Custom = -1,
    Primary = 0,
    Secondary = 1,
    White = 2,
    Black = 3,
    SupportingNeutral = 4,
    Accent = 5,
    ContentColor1 = 6
}

[CreateAssetMenu(menuName = "Flexible UI Data")]
public class FlexibleUIData : ScriptableObject
{
    [Header("Color Palette")]
    public Color32 primaryColor = new Color32(75,75,75,255);
    public Color32 secondaryColor = new Color32(155,155,155,255);
    public Color32 whiteColor = new Color32(255,255,255,255);
    public Color32 blackColor = new Color32(0,0,0,255);
    public Color32 supportingNeutralColor = new Color32(195,195,195,255);
    public Color32 accentColor = new Color32(234, 192, 0, 255);
    public Color32 contentColor1 = new Color32(75, 75, 75, 255);
    public TMP_ColorGradient primaryTextGradient;
    public TMP_ColorGradient secondaryTextGradient;

    public Color32 GetColor(ColorChoice colorChoice)
    {
        switch (colorChoice)
        {
            case ColorChoice.Primary:
                return primaryColor;
            case ColorChoice.Secondary:
                return secondaryColor;
            case ColorChoice.White:
                return whiteColor;
            case ColorChoice.Black:
                return blackColor;
            case ColorChoice.SupportingNeutral:
                return supportingNeutralColor;
            case ColorChoice.Accent:
                return accentColor;
            case ColorChoice.ContentColor1:
                return contentColor1;
            default:
                return Color.red;
        }
    }

    [Header("Buttons")]
    public Sprite primaryButtonSprite;
    public SpriteState primaryButtonSpriteState;
    public Sprite secondaryButtonSprite;
    public SpriteState secondaryButtonSpriteState;

    [Header("Toggles")]
    public Sprite checkmark;
    public Sprite toggleBackground;
    


    // [Header("Font Style")]
    // public TMP_FontAsset primaryFont;
    // public TMP_FontAsset secondaryFont;

    [Header("Typography")]
    public Typography primaryTypography;
    public Typography secondaryTypography;

    [Header("Panels")]

    public Sprite panelBackground;
    public Sprite panelTiling;
    public Sprite panelBorder;

    [Header("Lines")]
    public Sprite primaryLineGradient;
    public Sprite secondaryLineGradient;
    
}
