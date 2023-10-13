using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class FlexibleUIIcon : FlexibleUIBase
{
    MaskableGraphic graphic;
    public ImageColor imageColor;

    public enum ImageColor
    {
        White = 0,
        Black = 1,
        Primary = 2,
        Secondary = 3,
        SupportingNeutral = 4,
        Accent = 5,
        Custom = 6,
        ContentColor1 = 7
    }

    public override void Awake()
    {
        graphic = GetComponent<MaskableGraphic>();

        base.Awake();
    }

    protected override void OnSkinUI()
    {
        if (skinData == null) return;

        switch (imageColor)
        {
            case ImageColor.White:
                graphic.color = skinData.whiteColor;
                break;
            case ImageColor.Black:
                graphic.color = skinData.blackColor;
                break;
            case ImageColor.Primary:
                graphic.color = skinData.primaryColor;
                break;
            case ImageColor.Secondary:
                graphic.color = skinData.secondaryColor;
                break;
            case ImageColor.SupportingNeutral:
                graphic.color = skinData.supportingNeutralColor;
                break;
            case ImageColor.Accent:
                graphic.color = skinData.accentColor;
                break;
            case ImageColor.Custom:
                break;
            case ImageColor.ContentColor1:
                graphic.color = skinData.contentColor1;
                break;
        }

        base.OnSkinUI();
    }
}