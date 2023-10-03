using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.GenericUI.Accordion
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class AccordionGroup : BaseWindow
    {
        public RectTransform rt;
        public VerticalLayoutGroup verticalLayoutGroup;

        public AccordionElement[] accordionElements;

        /// <summary>
        /// If a single element is not collapsed, false. else true
        /// </summary>
        bool isCollapsed
        {
            get
            {
                if (accordionElements != null && accordionElements.Length > 0)
                {
                    foreach (AccordionElement accordionElement in accordionElements)
                    {
                        if (accordionElement.rt.rect.height > 1) { return false; }
                    }
                }
                return true;
            }
        }

        [SerializeField]
        private UIAnimationSequenceData collapseSequence;

        [SerializeField]
        private UIAnimationSequenceData expandSequence;

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

            base.Awake();
        }

        protected override void OnEnable()
        {
            if (onAwakeWindows != null)
                onAwakeWindows.AddListener(AwakeWindow);

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            if (onAwakeWindows != null)
                onAwakeWindows.RemoveListener(AwakeWindow);

            base.OnDisable();
        }

        private void AwakeWindow()
        {
            UpdateLayout();
        }

        public void UpdateLayout()
        {
            accordionElements = GetComponentsInChildren<AccordionElement>();

            verticalLayoutGroup.childControlHeight = true;

            foreach (AccordionElement accordionElement in accordionElements)
            {
                if (accordionElement.verticalLayoutGroup != null)
                    accordionElement.verticalLayoutGroup.childControlHeight = true;

                if (accordionElement.layoutElement != null && accordionElement.fullHeight > -1)
                    accordionElement.layoutElement.preferredHeight = accordionElement.fullHeight;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

            foreach (AccordionElement accordionElement in accordionElements)
            {
                if (accordionElement.rt != null)
                {
                      accordionElement.fullHeight = accordionElement.rt.rect.height;
                }

                if (accordionElement.verticalLayoutGroup != null)
                    accordionElement.verticalLayoutGroup.childControlHeight = false;

                if (accordionElement.layoutElement != null)
                    accordionElement.layoutElement.preferredHeight = -1;
            }

            verticalLayoutGroup.childControlHeight = false;
        }

       public void SwitchPivotToBottomCenter()
        {
            rt.SetPivot(PivotPresets.BottomCenter);
        }

        public void SwitchPivotToTopCenter()
        {
            rt.SetPivot(PivotPresets.TopCenter);
        }

        public void SetAnchorsToBottomCenter()
        {
            rt.SetAnchor(AnchorPresets.BottomCenter);
        }

        public void SetAnchorsToTopCenter()
        {
            rt.SetAnchor(AnchorPresets.TopCenter);
        }

        private void _Collapse(SequenceType sequenceType)
        {
            Debug.Log("collapse");
            _WindowAction(collapseSequence, sequenceType);
        }

        private void _Expand(SequenceType sequenceType)
        {
            Debug.Log("expand");
            _WindowAction(expandSequence, sequenceType);
        }


        public void Toggle(int s)
        {
            SequenceType sequenceType = (SequenceType)s;

            if (!isCollapsed)
                _Collapse(sequenceType);
            else
                _Expand(sequenceType);
        }
    }
}


