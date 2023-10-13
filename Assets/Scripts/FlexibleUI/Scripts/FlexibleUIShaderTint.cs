using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class FlexibleUIShaderTint : FlexibleUIBase
{
    private MeshRenderer meshRenderer;

    public TintColor tintColor;

    public enum TintColor
    {
        White,
        Black,
        Primary,
        Secondary,
        SupportingNeutral,
        Accent,
        Custom
    }

    public override void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        base.Awake();
    }

    protected override void OnSkinUI()
    {
        if (skinData != null)
        {
            switch (tintColor)
            {
                case TintColor.White:
                    meshRenderer.sharedMaterial.SetColor("_Color", skinData.whiteColor);
                    break;
                case TintColor.Black:
                    meshRenderer.sharedMaterial.SetColor("_Color", skinData.blackColor);
                    break;
                case TintColor.Primary:
                    meshRenderer.sharedMaterial.SetColor("_Color", skinData.primaryColor);
                    break;
                case TintColor.Secondary:
                    meshRenderer.sharedMaterial.SetColor("_Color", skinData.secondaryColor);
                    break;
                case TintColor.SupportingNeutral:
                    meshRenderer.sharedMaterial.SetColor("_Color", skinData.supportingNeutralColor);
                    break;
                case TintColor.Accent:
                    meshRenderer.sharedMaterial.SetColor("_Color", skinData.accentColor);
                    break;
                case TintColor.Custom:
                    break;
            }
        }

        base.OnSkinUI();
    }
}