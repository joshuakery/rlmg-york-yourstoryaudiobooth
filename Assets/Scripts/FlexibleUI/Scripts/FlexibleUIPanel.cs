using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlexibleUIPanel : FlexibleUIBase
{
    Image backgroundImage;
    Image tilingImage;
    Image borderImage;

    public Sprite borderOverride;

    public override void Awake()
    {
        backgroundImage = GetComponent<Image>();
        tilingImage = transform.GetChild(0).GetComponent<Image>();
        borderImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        base.Awake();
    }

    protected override void OnSkinUI()
    {
        backgroundImage.type = Image.Type.Simple;
        backgroundImage.sprite = skinData.panelBackground;

        tilingImage.type = Image.Type.Tiled;
        tilingImage.sprite = skinData.panelTiling;

        borderImage.type = Image.Type.Sliced;
        if (borderOverride != null)
        {
            borderImage.sprite = borderOverride;
        }   
        else
        {
            borderImage.sprite = skinData.panelBorder;
        }

    }
}