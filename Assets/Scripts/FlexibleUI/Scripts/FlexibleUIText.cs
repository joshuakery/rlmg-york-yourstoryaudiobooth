using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FlexibleUIText : FlexibleUIBase
{
    protected TMP_Text tmp_text;

    public ScreenType screenType;

    public HeadingType headingType;
    public FontColor fontColor;

    public enum ScreenType
    {
        SecondaryMonitor,
        PrimaryTouchscreen,
        
    }

    public enum HeadingType
    {
        Title,
        Subtitle,
        H1,
        H2,
        H3,
        H4,
        P,
        Button,
        Caption,
        Custom
    }

    public enum FontColor
    {
        White,
        Black,
        Primary,
        Secondary,
        SupportingNeutral,
        Accent,
        PrimaryGradient,
        SecondaryGradient,
        Custom,
        ContentColor1
    }

    public override void Awake()
    {
        tmp_text = GetComponent<TMP_Text>();

        base.Awake();
    }

    private void SetStyling(TextType textType)
    {
        tmp_text.font = textType.font;
        tmp_text.fontWeight = textType.weight;
        tmp_text.fontSize = textType.size;
        //Character Spacing
        tmp_text.characterSpacing = textType.spacingOptions.character;
        tmp_text.wordSpacing = textType.spacingOptions.word;
        tmp_text.lineSpacing = textType.spacingOptions.line;
        tmp_text.paragraphSpacing = textType.spacingOptions.paragraph;
    }

    private void HeadingTypeSwitch(Typography typography)
    {
        switch (headingType)
        {
            case HeadingType.Title:
                SetStyling(typography.title);
                break;
            case HeadingType.Subtitle:
                SetStyling(typography.subtitle);
                break;
            case HeadingType.H1:
                SetStyling(typography.h1);
                break;
            case HeadingType.H2:
                SetStyling(typography.h2);
                break;
            case HeadingType.H3:
                SetStyling(typography.h3);
                break;
            case HeadingType.H4:
                SetStyling(typography.h4);
                break;
            case HeadingType.P:
                SetStyling(typography.p);
                break;
            case HeadingType.Caption:
                SetStyling(typography.caption);
                break;
            case HeadingType.Button:
                break;
            case HeadingType.Custom:
                break;
        }
    }

    protected override void OnSkinUI()
    {
        switch(screenType)
        {
            case ScreenType.PrimaryTouchscreen:
                HeadingTypeSwitch(skinData.primaryTypography);
                break;

            case ScreenType.SecondaryMonitor:
                HeadingTypeSwitch(skinData.secondaryTypography);
                break;
        }

        switch(fontColor)
        {
            case FontColor.White:
                tmp_text.color = skinData.whiteColor;
                tmp_text.enableVertexGradient = false;
                break;
            case FontColor.Black:
                tmp_text.color = skinData.blackColor;
                tmp_text.enableVertexGradient = false;
                break;
            case FontColor.Primary:
                tmp_text.color = skinData.primaryColor;
                tmp_text.enableVertexGradient = false;
                break;
            case FontColor.Secondary:
                tmp_text.color = skinData.secondaryColor;
                tmp_text.enableVertexGradient = false;
                break;
            case FontColor.SupportingNeutral:
                tmp_text.color = skinData.supportingNeutralColor;
                tmp_text.enableVertexGradient = false;
                break;
            case FontColor.Accent:
                tmp_text.color = skinData.accentColor;
                tmp_text.enableVertexGradient = false;
                break;
            case FontColor.PrimaryGradient:
                tmp_text.color = Color.white;
                tmp_text.enableVertexGradient = true;
                tmp_text.colorGradientPreset = skinData.primaryTextGradient;
                break;
            case FontColor.SecondaryGradient:
                tmp_text.color = Color.white;
                tmp_text.enableVertexGradient = true;
                tmp_text.colorGradientPreset = skinData.secondaryTextGradient;
                break;
            case FontColor.Custom:
                break;
            case FontColor.ContentColor1:
                tmp_text.color = skinData.contentColor1;
                tmp_text.enableVertexGradient = false;
                break;
        }

        base.OnSkinUI();
    }
}