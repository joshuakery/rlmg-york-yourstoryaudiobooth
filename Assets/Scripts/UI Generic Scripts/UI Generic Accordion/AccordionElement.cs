using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using JoshKery.GenericUI.DOTweenHelpers;
using DG.Tweening;

namespace JoshKery.GenericUI.Accordion
{
    [RequireComponent(typeof(RectTransform))]
    public class AccordionElement : BaseWindow
    {
        public float fullHeight = -1f;

        public RectTransform rt;

        public VerticalLayoutGroup verticalLayoutGroup;

        public LayoutElement layoutElement;

        protected override void Awake()
        {
            if (rt == null)
            {
                rt = GetComponent<RectTransform>();
            }

            if (verticalLayoutGroup == null)
            {
                verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();

                
            }

            if (layoutElement == null)
                layoutElement = GetComponent<LayoutElement>();

            base.Awake();
        }
    }
}


