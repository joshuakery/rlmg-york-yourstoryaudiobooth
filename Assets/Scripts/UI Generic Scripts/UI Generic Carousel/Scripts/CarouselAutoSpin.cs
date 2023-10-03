using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JoshKery.GenericUI.Carousel
{
    public class CarouselAutoSpin : MonoBehaviour
    {
        [SerializeField]
        private Carousel carousel;

        /// <summary>
        /// Whether the carousel spins; functions monitoring user input will still be called
        /// </summary>
        public bool doSpin = true;

        /// <summary>
        /// Duration of no user input before auto spin start up
        /// </summary>
        public float timeout = 60f;

        /// <summary>
        /// Time in seconds between carousel spin to next/prev slide
        /// </summary>
        public float spinInterval = 10f;

        public enum CarouselDirection
        {
            Backward = 0,
            Forward = 1
        }

        /// <summary>
        /// Direction of carousel spin i.e. next or prev slide
        /// </summary>
        public CarouselDirection carouselDirection = CarouselDirection.Forward;

        bool anyClick = false;
        float timeSinceLastClick = 0.0f;

        bool timedOut = false;

        /// <summary>
        /// Hook for auto carousel start up
        /// </summary>
        private UnityEvent _onAutoCarouselStart;
        public UnityEvent onAutoCarouselStart
        {
            get
            {
                if (_onAutoCarouselStart == null)
                    _onAutoCarouselStart = new UnityEvent();

                return _onAutoCarouselStart;
            }
        }

        void Update()
        {
            if (!doSpin)
            {
                StopAllCoroutines();
            }

            timeSinceLastClick += Time.deltaTime;

            if (Input.anyKey ||
                Input.GetMouseButton(0) ||
                Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                anyClick = true;
            }

            if (anyClick)
            {
                StopAllCoroutines();

                timeSinceLastClick = 0.0f;
                anyClick = false;
            }

            if (timeSinceLastClick > timeout && !timedOut && doSpin)
            {
                onAutoCarouselStart.Invoke();
                if (carousel.sequenceManager != null) { carousel.sequenceManager.CreateNewSequenceAfterCurrent(); }
                StartCoroutine(SpinCarousel());

                timedOut = true;
            }
            else if (timeSinceLastClick < timeout && timedOut)
            {
                timedOut = false;
            }
        }

        private IEnumerator SpinCarousel()
        {
            while (true)
            {
                if (doSpin && spinInterval > 0)
                {
                    yield return new WaitForSeconds(spinInterval);

                    if (carousel != null)
                    {
                        switch(carouselDirection)
                        {
                            case CarouselDirection.Backward:
                                carousel.PrevSlide();
                                break;
                            case CarouselDirection.Forward:
                                carousel.NextSlide();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }

        #region Helper Methods
        public void StopCarousel()
        {
            doSpin = false;
        }

        public void StartCarousel()
        {
            doSpin = true;
        }
        #endregion
    }
}


