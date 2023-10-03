using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.GenericUI.AspectRatio
{
    [RequireComponent(typeof(AspectRatioFitter))]
    [RequireComponent(typeof(RawImageManager))]
    public class AspectRatioFitterRawImageManagerSubscriber : MonoBehaviour
    {
        private AspectRatioFitter aspectRatioFitter;
        private RawImageManager riManager;

        private void Awake()
        {
            aspectRatioFitter = GetComponent<AspectRatioFitter>();
            riManager = GetComponent<RawImageManager>();
        }

        private void OnEnable()
        {
            if (riManager != null)
                riManager.onAspectRatioChanged.AddListener(SetAspectRatio);
        }

        private void OnDisable()
        {
            if (riManager != null)
                riManager.onAspectRatioChanged.RemoveListener(SetAspectRatio);
        }

        private void SetAspectRatio(float ratio)
        {
            aspectRatioFitter.aspectRatio = ratio;
        }
    }
}


