using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class GenericPage : BaseWindow
    {
        [SerializeField]
        protected PageManager.Page pageType;

        private PageManager pageManager;

        protected override void Awake()
        {
            base.Awake();

            if (pageManager == null)
                pageManager = FindObjectOfType<PageManager>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            if (pageManager != null)
                pageManager.onNewPage += OnNewPage;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (pageManager != null)
                pageManager.onNewPage -= OnNewPage;
        }

        protected virtual void OnNewPage(PageManager.Page page)
        {
            if (page == pageType)
                Open(0f);
            else
                Close(0f);
        }
    }
}


