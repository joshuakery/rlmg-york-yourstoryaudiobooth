using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    [RequireComponent(typeof(RectTransform))]
    public class AudioVizHideElement : MonoBehaviour
    {
        public enum Side
        {
            Right,
            Left
        }

        [SerializeField]
        private Side side;

        private RectTransform rt;

        [SerializeField]
        private RectTransform areaToFill;

        [SerializeField]
        private RectTransform fillUpTo;

        [SerializeField]
        private RectTransform fillUpToDisplacement;

        private float nw;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (areaToFill != null && fillUpTo != null && fillUpToDisplacement != null)
            {
                if (side == Side.Right)
                {
                    float halfWidth = (areaToFill.rect.width / 2f);
                    float sliderOffset = halfWidth + fillUpToDisplacement.anchoredPosition.x;
                    float borderOffset = sliderOffset + fillUpTo.anchoredPosition.x;
                    float borderWidthOffset = borderOffset + (fillUpTo.rect.width / 2f);
                    nw = areaToFill.rect.width - borderWidthOffset;

                    //nw = ((areaToFill.rect.width - fillUpTo.rect.width) / 2f) + fillUpToDisplacement.anchoredPosition.x;
                    rt.sizeDelta = new Vector2(nw, areaToFill.rect.height);
                }
                else if (side == Side.Left)
                {
                    float halfWidth = (areaToFill.rect.width / 2f);
                    float sliderOffset = halfWidth + fillUpToDisplacement.anchoredPosition.x;
                    float borderOffset = sliderOffset + fillUpTo.anchoredPosition.x;
                    float borderWidthOffset = borderOffset - (fillUpTo.rect.width / 2f);
                    nw = borderWidthOffset;

                    //nw = ((areaToFill.rect.width - fillUpTo.rect.width) / 2f) - fillUpToDisplacement.anchoredPosition.x;
                    rt.sizeDelta = new Vector2(nw, areaToFill.rect.height);
                }
            }
        }
    }
}


