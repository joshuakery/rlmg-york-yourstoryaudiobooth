using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using JoshKery.GenericUI.DOTweenHelpers;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PlaybackTrimSwitchManager : MonoBehaviour
    {
        [SerializeField]
        private PlaybackUIManager playbackManager;

        [SerializeField]
        private UISequenceManager sequenceManager;

        [SerializeField]
        private BaseWindow playbackHandle;

        [SerializeField]
        private PlaybackHeadWindow playbackHeadWindow;

        [SerializeField]
        private BaseWindow rightHandleWindow;

        [SerializeField]
        private BaseWindow leftHandleWindow;

        [SerializeField]
        private MinMaxSlider trimSlider;

        [SerializeField]
        private AudioVizWindow audioVizWindow;

        [SerializeField]
        private BaseWindow rightCoverWindow;

        [SerializeField]
        private BaseWindow leftCoverWindow;

        [SerializeField]
        private BaseWindow measurementDisplayWindow;

        [SerializeField]
        private MeasurementDisplay measurementDisplay;

        [SerializeField]
        private SliderMiddleGraphicWindow middleGraphicWindow;

        [SerializeField]
        private BaseWindow borderFadeWindow;

        public bool isPlayback = true;

        public delegate void OnSwitchToTrimEvent();
        public OnSwitchToTrimEvent onTrim;

        public delegate void OnSwitchToPlaybackEvent();
        public OnSwitchToPlaybackEvent onPlayback;

        private bool trimSliderIsUnTrimmed
        {
            get
            {
                if (trimSlider != null)
                    return trimSlider.Values.minValue == 0 && trimSlider.Values.maxValue == 1;
                else
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doSwitchToPlaybackMode"></param>
        public void Switch(bool doSwitchToPlaybackMode)
        {
            isPlayback = doSwitchToPlaybackMode;

            if (playbackManager != null)
            {
                if (trimSlider != null)
                {
                    playbackManager.minValue = trimSlider.Values.minValue;
                    playbackManager.maxValue = trimSlider.Values.maxValue;
                }

                if (doSwitchToPlaybackMode) //Pause AND start start to min
                {
                    playbackManager.Init();
                }
                else //Only pause here so we don't jump the slider around
                    playbackManager.PauseAudio();

            }

/*            if (playbackHeadWindow != null && doSwitchToPlaybackMode)
                playbackHeadWindow.Close();*/

            if (sequenceManager != null)
            {
                sequenceManager.KillCurrentSequence();
            }

            if (doSwitchToPlaybackMode)
            {
                if (rightHandleWindow != null)
                    rightHandleWindow.Close(0f);

                if (leftHandleWindow != null)
                    leftHandleWindow.Close(0f);

                if (leftCoverWindow != null)
                    leftCoverWindow.Open(0f);

                if (rightCoverWindow != null)
                    rightCoverWindow.Open(0f);

                if (trimSliderIsUnTrimmed)
                {
                    if (middleGraphicWindow != null)
                        middleGraphicWindow.Close(0f);

                    if (audioVizWindow != null)
                        audioVizWindow.Close(0f);

                    if (borderFadeWindow != null)
                        borderFadeWindow.Close(0f);

                    if (playbackHandle != null)
                        playbackHandle.Open(0f);
                }
                else
                {
                    if (measurementDisplayWindow != null)
                        measurementDisplayWindow.Close(0f);

                    if (middleGraphicWindow != null)
                        middleGraphicWindow.Close(0.5f);

                    if (audioVizWindow != null)
                        audioVizWindow.Close(0.5f);

                    if (borderFadeWindow != null)
                        borderFadeWindow.Close(0.9f);

                    if (playbackHandle != null)
                        playbackHandle.Open(0.9f);

                    if (measurementDisplay != null)
                        sequenceManager.InsertCallback(0.9f, () => { measurementDisplay.UpdateDisplay(); });

                    if (measurementDisplayWindow != null)
                        measurementDisplayWindow.Open(0.9f);
                }

                

                onPlayback?.Invoke();
            }
            else
            {
                if (playbackHandle != null)
                    playbackHandle.Close(0f);

                if (borderFadeWindow != null)
                    borderFadeWindow.Open(0f);

                if (leftCoverWindow != null)
                    leftCoverWindow.Close(0f);

                if (rightCoverWindow != null)
                    rightCoverWindow.Close(0f);

                if (trimSliderIsUnTrimmed)
                {
                    if (middleGraphicWindow != null)
                        middleGraphicWindow.Open(0f);

                    if (audioVizWindow != null)
                        audioVizWindow.Open(0f);

                    if (rightHandleWindow != null)
                        rightHandleWindow.Open(0f);

                    if (leftHandleWindow != null)
                        leftHandleWindow.Open(0f);
                }
                else
                {
                    if (measurementDisplayWindow != null)
                        measurementDisplayWindow.Close(0f);

                    if (middleGraphicWindow != null)
                        middleGraphicWindow.Open(0.5f);

                    if (audioVizWindow != null)
                        audioVizWindow.Open(0.5f);

                    if (rightHandleWindow != null)
                        rightHandleWindow.Open(0.7f);

                    if (leftHandleWindow != null)
                        leftHandleWindow.Open(0.7f);

                    if (measurementDisplay != null)
                        sequenceManager.InsertCallback(0.7f, () => { measurementDisplay.UpdateDisplay(); });

                    if (measurementDisplayWindow != null)
                        measurementDisplayWindow.Open(0.7f);
                }

                onTrim?.Invoke();
            }
        }

        public void OnSwitch()
        {
            //Toggle
            Switch(!isPlayback);
        }
    }
}

