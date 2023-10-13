using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class FlexibleUIButton : FlexibleUIBase
{

    Button button;
    Image image;

    public ButtonStyle buttonStyle;
    public ImageColor imageColor;

    public enum ButtonStyle
    {
        Primary,
        Secondary,
        Custom
    }

    public enum ImageColor
    {
        White,
        Black,
        Primary,
        Secondary,
        Custom
    }

    public override void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        base.Awake();
    }

    protected override void OnSkinUI()
    {
        switch (buttonStyle)
        {
            case ButtonStyle.Primary:
                // button.transition = Selectable.Transition.SpriteSwap;
                button.transition = Selectable.Transition.ColorTint;
                button.targetGraphic = image;
                image.sprite = skinData.primaryButtonSprite;
                image.type = Image.Type.Sliced;
                button.spriteState = skinData.primaryButtonSpriteState;
                break;
            case ButtonStyle.Secondary:
                // button.transition = Selectable.Transition.SpriteSwap;
                button.transition = Selectable.Transition.ColorTint;
                button.targetGraphic = image;
                image.sprite = skinData.secondaryButtonSprite;
                image.type = Image.Type.Sliced;
                button.spriteState = skinData.secondaryButtonSpriteState;
                break;
            case ButtonStyle.Custom:
                break;
        }

        switch (imageColor)
        {
            case ImageColor.White:
                image.color = skinData.whiteColor;
                break;
            case ImageColor.Black:
                image.color = skinData.blackColor;
                break;
            case ImageColor.Primary:
                image.color = skinData.primaryColor;
                break;
            case ImageColor.Secondary:
                image.color = skinData.secondaryColor;
                break;
            case ImageColor.Custom:
                break;
        }

        
        



        base.OnSkinUI();
    }
}