using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FlexibleUIBase : MonoBehaviour
{
    public delegate void AllOnSkinUI();
    public static AllOnSkinUI allOnSkinUI;

    public FlexibleUIData skinData;

    protected virtual void OnSkinUI()
    {

    }

    public virtual void Awake()
    {
        OnSkinUI();
    }

    private void OnEnable()
    {
            allOnSkinUI += OnSkinUI;
    }

    private void OnDisable()
    {
            allOnSkinUI -= OnSkinUI;
    }


    public virtual void Update()
    {
        if(Application.isEditor && !Application.isPlaying)
        {
            OnSkinUI();
        }
    }

}