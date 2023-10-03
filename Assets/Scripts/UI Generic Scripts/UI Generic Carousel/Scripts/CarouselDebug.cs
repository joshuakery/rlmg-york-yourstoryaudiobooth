using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshKery.GenericUI.Carousel
{
    [RequireComponent(typeof(Carousel))]
    public class CarouselDebug : MonoBehaviour
    {
        private Carousel carousel;

        [SerializeField]
        private bool doDebug = false;

        private void Awake()
        {
            carousel = GetComponent<Carousel>();
        }

        private void Update()
        {
            if (doDebug)
                HandleDebugInput();
        }

        public void HandleDebugInput()
        {
            if (carousel != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    carousel.NextSlide();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    carousel.PrevSlide();
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (carousel.slideManager.slideOrder.Count > 0)
                        carousel.GoToSlide(0);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (carousel.slideManager.slideOrder.Count > 1)
                        carousel.GoToSlide(1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    if (carousel.slideManager.slideOrder.Count > 2)
                        carousel.GoToSlide(2);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    if (carousel.slideManager.slideOrder.Count > 3)
                        carousel.GoToSlide(3);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    if (carousel.slideManager.slideOrder.Count > 4)
                        carousel.GoToSlide(4);
                }
            }
        }

        public void SetDoDebug(bool value)
        {
            doDebug = value;
        }
    }
}


