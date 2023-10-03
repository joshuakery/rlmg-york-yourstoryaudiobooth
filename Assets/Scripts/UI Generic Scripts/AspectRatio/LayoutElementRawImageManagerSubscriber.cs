using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.GenericUI.AspectRatio
{
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(RawImageManager))]
    public class LayoutElementRawImageManagerSubscriber : MonoBehaviour
    {
        private LayoutElement le;
        private RawImageManager riManager;

        [SerializeField]
        private AspectRatioFitter.AspectMode aspectMode;

        private void Awake()
        {
            le = GetComponent<LayoutElement>();
            riManager = GetComponent<RawImageManager>();
        }

        private void OnEnable()
        {
            if (riManager != null)
                riManager.onSizeChanged.AddListener(SetPreferredLayout);
        }

        private void OnDisable()
        {
            if (riManager != null)
                riManager.onSizeChanged.RemoveListener(SetPreferredLayout);
        }

        private void SetPreferredLayout(Vector2 size)
        {
            if (le != null)
            {
                float ratio = (float)size.x / (float)size.y;

                switch (aspectMode)
                {
                    case AspectRatioFitter.AspectMode.HeightControlsWidth:
                        le.preferredWidth = le.preferredHeight * ratio;
                        break;
                    case AspectRatioFitter.AspectMode.WidthControlsHeight:
                        le.preferredHeight = le.preferredWidth * ( 1f / ratio );
                        break;
                    case AspectRatioFitter.AspectMode.FitInParent:
                        float parentRatio = (float)le.preferredWidth / (float)le.preferredHeight;
                        if (ratio > parentRatio)
                            le.preferredHeight = le.preferredWidth * (1f / ratio);
                        else if (ratio < parentRatio)
                            le.preferredWidth = le.preferredHeight * ratio;
                        break;
                    default:
                        le.preferredWidth = size.x;
                        le.preferredHeight = size.y;
                        break;
                }

                
            }
        }
    }
}
