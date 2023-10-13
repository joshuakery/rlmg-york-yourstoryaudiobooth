using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Toggle))]
public class FlexibleUIToggle : FlexibleUIBase
{
    public Toggle toggle;
    public Image background;
    public Image checkmark;

    public override void Awake()
    {
        toggle = GetComponent<Toggle>();
        
        base.Awake();
    }

    protected override void OnSkinUI()
    {
        if (background != null)
            background.sprite = skinData.toggleBackground;

        if (checkmark != null)
            checkmark.sprite = skinData.checkmark;
        

        base.OnSkinUI();
    }
}