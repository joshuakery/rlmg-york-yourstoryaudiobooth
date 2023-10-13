using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlexibleUIBorder : FlexibleUIBase
{
    public enum BorderColor
    {
        PrimaryGradient,
        SecondaryGradient,
        White,
        Black,
        Primary,
        Secondary,
        Custom
        
    }

    Image image;

    public BorderColor borderColor;

    public override void Awake()
    {
        image = GetComponent<Image>();

        base.Awake();
    }

    protected override void OnSkinUI()
    {
        image.type = Image.Type.Simple;

        switch(borderColor)
        {
            case BorderColor.PrimaryGradient:
                image.sprite = skinData.primaryLineGradient;
                break;
            case BorderColor.SecondaryGradient:
                image.sprite = skinData.secondaryLineGradient;
                break;
            case BorderColor.White:
                image.sprite = null;
                image.color = skinData.whiteColor;
                break;
            case BorderColor.Black:
                image.sprite = null;
                image.color = skinData.blackColor;
                break;
            case BorderColor.Primary:
                image.sprite = null;
                image.color = skinData.primaryColor;
                break;
            case BorderColor.Secondary:
                image.sprite = null;
                image.color = skinData.secondaryColor;
                break;
            case BorderColor.Custom:
                break;
        }

        
    }
}