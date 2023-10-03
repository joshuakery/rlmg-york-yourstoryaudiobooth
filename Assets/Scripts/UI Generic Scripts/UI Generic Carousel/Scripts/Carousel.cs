using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using JoshKery.GenericUI.DOTweenHelpers;
using JoshKery.GenericUI.Events;

namespace JoshKery.GenericUI.Carousel
{
    public class Carousel : MonoBehaviour
    {
        #region FIELDS
        public int CurrentSlideIndex { get; protected set; } = -1;

        public SlideManager slideManager;
        public Carousel navbar;

        public bool doLoop = false;

        public UISequenceManager sequenceManager;

        /// <summary>
        /// Gets id used in slideOrder List of slideManager
        /// </summary>
        public string CurrentSlideID
        {
            get
            {
                return slideManager?.GetCurrentSlideID(CurrentSlideIndex);
            }
        }
        #endregion

        #region Events
        private IntEvent _onSlideChanged;
        public IntEvent onSlideChanged
        {
            get
            {
                if (_onSlideChanged == null)
                    _onSlideChanged = new IntEvent();

                return _onSlideChanged;
            }
        }
        #endregion

        protected virtual void OnEnable()
        {
            if (slideManager != null)
                slideManager.onInitialized.AddListener(Setup);
        }

        protected virtual void OnDisable()
        {
            if (slideManager != null)
                slideManager.onInitialized.RemoveListener(Setup);
        }

        /// <summary>
        /// Subscribed to slideManager.onInitialized, resets carousel to present first slide
        /// </summary>
        public virtual void Setup()
        {
            SlideOutAll();
            GoToFirstSlide();
            sequenceManager.CompleteCurrentSequence();
        }

        /// <summary>
        /// Trigger a transition out animation for all slides
        /// </summary>
        public virtual void SlideOutAll()
        {
            foreach (KeyValuePair<string, SlideDisplay> kvp in slideManager.slideDisplays)
            {
                SlideDisplay display = kvp.Value;
                display.SlideOutForPrev();
            }

            if (navbar != null)
            {
                navbar.SlideOutAll();
            }
        }

        /// <summary>
        /// Go to the first slide
        /// </summary>
        public void GoToFirstSlide()
        {
            int _currentSlideIndex = CurrentSlideIndex;
            int firstSlideIndex = 0;

            GoToSlide(_currentSlideIndex, firstSlideIndex, ForceDirection.NewIsNext);
        }

        /// <summary>
        /// Go to the next slide
        /// </summary>
        public void NextSlide()
        {
            int nextSlideIndex = (CurrentSlideIndex + 1);
            ForceDirection forceDirection = ForceDirection.Default;

            if (doLoop)
            {
                if (nextSlideIndex >= slideManager.slideDisplays.Count)
                {
                    nextSlideIndex = 0;
                    forceDirection = ForceDirection.NewIsNext;
                }
            }

            if (nextSlideIndex >= slideManager.slideDisplays.Count) { return; }

            int _currentSlideIndex = CurrentSlideIndex;

            GoToSlide(_currentSlideIndex, nextSlideIndex, forceDirection);
        }

        /// <summary>
        /// Go to previous slide
        /// </summary>
        public void PrevSlide()
        {
            int prevSlideIndex = (CurrentSlideIndex - 1);
            ForceDirection forceDirection = ForceDirection.Default;

            if (doLoop)
            {
                if (prevSlideIndex < 0)
                {
                    prevSlideIndex = slideManager.slideDisplays.Count - 1;
                    forceDirection = ForceDirection.NewIsPrev;
                }
            }


            if (prevSlideIndex < 0) { return; }

            int _currentSlideIndex = CurrentSlideIndex;

            GoToSlide(_currentSlideIndex, prevSlideIndex, forceDirection);
        }

        /// <summary>
        /// Go to the slide at the given index, if valid.
        /// </summary>
        /// <param name="newSlideIndex"></param>
        public void GoToSlide(int newSlideIndex)
        {
            if (CurrentSlideIndex == newSlideIndex) { return; }
            if (newSlideIndex < 0 || newSlideIndex > slideManager.slideDisplays.Count - 1) { return; }

            int _currentSlideIndex = CurrentSlideIndex;

            GoToSlide(_currentSlideIndex, newSlideIndex);
        }

        /// <summary>
        /// "Direction" slides should animate, used to override Default order in slideManager.slidesOrder;
        /// Should the new slide move as if it were next in the sequence, or previous?
        /// </summary>
        public enum ForceDirection
        {
            Default = 0,
            NewIsNext = 1,
            NewIsPrev = -1
        }


        /// <summary>
        /// Triggers the animations for transitioning from the old slide to the new slide.
        /// </summary>
        /// <param name="oldSlideIndex">Slide index to transition from.</param>
        /// <param name="newSlideIndex">Slide index to transition to.</param>
        /// <param name="forceDirection">Direction slides should animate.</param>
        /// <param name="doCompleteCurrentSequence">If true, current sequence completes before creating new tweens</param>
        public virtual void GoToSlide(
            int oldSlideIndex, int newSlideIndex,
            ForceDirection forceDirection = ForceDirection.Default,
            bool doCompleteCurrentSequence = true
        )
        {
            if (doCompleteCurrentSequence && sequenceManager != null)
                sequenceManager.CompleteCurrentSequence();

            for (int i = 0; i < slideManager.slideOrder.Count; i++)
            {
                string id = slideManager.slideOrder[i];
                if (slideManager.slideDisplays.ContainsKey(id))
                {
                    SlideDisplay display = slideManager.slideDisplays[id];

                    if (forceDirection == ForceDirection.NewIsNext ||
                        (forceDirection == ForceDirection.Default && newSlideIndex > oldSlideIndex)
                    ) //new should enter as next; old should exit for next
                    {
                        if (i == oldSlideIndex && i != newSlideIndex) { display.SlideOutForNext(); }
                        if (i == newSlideIndex) { display.SlideInAsNext(); }
                    }
                    else if (forceDirection == ForceDirection.NewIsPrev ||
                             (forceDirection == ForceDirection.Default && newSlideIndex < oldSlideIndex)
                    ) //new should enter as prev; old should exit for prev
                    {
                        if (i == oldSlideIndex && i != newSlideIndex) { display.SlideOutForPrev(); }
                        if (i == newSlideIndex) { display.SlideInAsPrev(); }
                    }
                }
            }

            if (navbar != null)
            {
                navbar.GoToSlide(oldSlideIndex, newSlideIndex, forceDirection, false);
            }

            CurrentSlideIndex = newSlideIndex;

            onSlideChanged.Invoke(CurrentSlideIndex);
        }
    }
}

