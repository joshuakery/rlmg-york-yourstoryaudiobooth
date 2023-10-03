using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace JoshKery.GenericUI.Carousel
{
    public class SlideManager : BaseDisplay
    {
        #region Slide Tracking Dict and List
        private Dictionary<string, SlideDisplay> _slideDisplays;
        public Dictionary<string, SlideDisplay> slideDisplays
        {
            get
            {
                if (_slideDisplays == null)
                    _slideDisplays = new Dictionary<string, SlideDisplay>();

                return _slideDisplays;
            }
            set
            {
                slideDisplays = value;
            }
        }

        private List<string> _slideOrder;
        public List<string> slideOrder
        {
            get
            {
                if (_slideOrder == null)
                    _slideOrder = new List<string>();

                return _slideOrder;
            }
            set
            {
                _slideOrder = value;
            }
        }
        #endregion

        #region FIELDS
        [SerializeField]
        protected SlideManager navbarManager;

        [SerializeField]
        private bool doUseExistingChildren = false;
        #endregion

        #region Events
        public UnityEvent onInitialized;
        #endregion

        #region Monobehaviour Methods
        protected override void Awake()
        {
            if (!doUseExistingChildren)
                base.Awake();
        }

        private void Start()
        {
            if (doUseExistingChildren)
                InitializeWithExistingChildren();
        }
        #endregion

        #region Initialization
        public void InstantiateSlideDisplays(int n)
        {
            ClearAllDisplays();

            for(int i=0; i<n; i++)
            {
                SlideDisplay display = InstantiateDisplay<SlideDisplay>();
                slideDisplays[i.ToString()] = display;
                slideOrder.Add(i.ToString());
            }

            if (navbarManager != null)
            {
                navbarManager.ClearAllDisplays();
                navbarManager.InstantiateSlideDisplays(n);
            }

            onInitialized.Invoke();
        }

        public override void ClearAllDisplays()
        {
            base.ClearAllDisplays();

            slideDisplays.Clear();
            slideOrder.Clear();
        }

        /// <summary>
        /// We can optionally define all the slides in the carousel in the Editor
        /// and initialize them here.
        /// </summary>
        public void InitializeWithExistingChildren()
        {
            slideDisplays.Clear();
            slideOrder.Clear();

            if (childDisplays != null)
            {
                SlideDisplay[] displays = childDisplays.Select(c => c.gameObject.GetComponent<SlideDisplay>()).ToArray();

                for (int i = 0; i < displays.Length; i++)
                {
                    SlideDisplay display = displays[i];
                    slideDisplays[i.ToString()] = display;
                    slideOrder.Add(i.ToString());
                }

                if (navbarManager != null)
                {
                    navbarManager.ClearAllDisplays();
                    navbarManager.InstantiateSlideDisplays(displays.Length);
                }
            }

            onInitialized.Invoke();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets an id string from slideOrder
        /// </summary>
        /// <param name="currentSlideIndex">Index of id to get</param>
        /// <returns></returns>
        public string GetCurrentSlideID(int currentSlideIndex)
        {
            if (currentSlideIndex >= 0 && currentSlideIndex < slideOrder.Count)
            {
                return slideOrder[currentSlideIndex];
            }
            else
            {
                return null;
            }
        }
        #endregion


    }
}


