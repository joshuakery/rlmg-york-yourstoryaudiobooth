using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using JoshKery.GenericUI.DOTweenHelpers;
using DG.Tweening;

namespace JoshKery.York.AudioRecordingBooth
{
    public class PlaybackTrimSwitchManager : MonoBehaviour
    {
        private AudioRecorderProcess audioRecorderProcess;

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
        private float trimSliderCachedMin = 0f;

        [SerializeField]
        private float trimSliderCachedMax = 1f;

        [SerializeField]
        private UIAnimationData trimSliderResetAnimationData;

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

        public delegate void OnCancelTrimEvent();
        public OnCancelTrimEvent onCancel;

        public delegate void OnSwitchStartEvent(bool isToPlayback);
        public OnSwitchStartEvent onSwitchStart;

        public delegate void OnSwitchCompleteEvent(bool isPlayback);
        public OnSwitchCompleteEvent onSwitchComplete;

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

        private bool trimSliderIsUnchanged
        {
            get
            {
                if (trimSlider != null)
                    return trimSlider.Values.minValue == trimSliderCachedMin && trimSlider.Values.maxValue == trimSliderCachedMax;
                else
                    return false;
            }
        }

        private void Awake()
        {
            audioRecorderProcess = FindObjectOfType<AudioRecorderProcess>();
        }

        private void OnEnable()
        {
            if (audioRecorderProcess != null)
                audioRecorderProcess.onStartRequested += OnAudioRecorderStartRequested;
        }

        private void OnDisable()
        {
            if (audioRecorderProcess != null)
                audioRecorderProcess.onStartRequested -= OnAudioRecorderStartRequested;
        }

        private void OnAudioRecorderStartRequested()
        {
            trimSliderCachedMin = 0;
            trimSliderCachedMax = 1;
        }

        private Tween ResetTrim(bool doCompleteImmediately = false)
        {
            if (trimSlider == null) return null;

            if (doCompleteImmediately)
            {
                onCancel?.Invoke();
                trimSlider.SetValues(trimSliderCachedMin, trimSliderCachedMax);
                return null;
            }

            Sequence wrapper = DOTween.Sequence();

            wrapper.InsertCallback(0f, () => { onCancel?.Invoke(); });

            Tween minTween = DOTween.To(
                    () => trimSlider.Values.minValue,
                    (x) => trimSlider.SetValues(x, trimSlider.Values.maxValue),
                    trimSliderCachedMin,
                    trimSliderResetAnimationData.duration
                );
            minTween.SetEase(trimSliderResetAnimationData.ease);
            wrapper.Join(minTween);

            Tween maxTween = DOTween.To(
                () => trimSlider.Values.maxValue,
                (x) => trimSlider.SetValues(trimSlider.Values.minValue, x),
                trimSliderCachedMax,
                trimSliderResetAnimationData.duration
            );
            maxTween.SetEase(trimSliderResetAnimationData.ease);
            wrapper.Join(maxTween);

            return wrapper;
        }

        private Tween SwitchToPlayback()
        {
            Sequence wrapper = DOTween.Sequence();

            wrapper.InsertCallback(0f, () => {
                trimSliderCachedMin = trimSlider.Values.minValue;
                trimSliderCachedMax = trimSlider.Values.maxValue;

                //Pause AND start start to min
                playbackManager.Init();
            });

            if (rightHandleWindow != null)
                wrapper.Insert(0f, rightHandleWindow.Close(SequenceType.UnSequenced));

            if (leftHandleWindow != null)
                wrapper.Insert(0f, leftHandleWindow.Close(SequenceType.UnSequenced));

            if (leftCoverWindow != null)
                wrapper.Insert(0f, leftCoverWindow.Open(SequenceType.UnSequenced));

            if (rightCoverWindow != null)
                wrapper.Insert(0f, rightCoverWindow.Open(SequenceType.UnSequenced));

            if (trimSliderIsUnTrimmed)
            {
                if (middleGraphicWindow != null)
                    wrapper.Insert(0f, middleGraphicWindow.Close(SequenceType.UnSequenced));

                if (audioVizWindow != null)
                    wrapper.Insert(0f, audioVizWindow.Close(SequenceType.UnSequenced));

                if (borderFadeWindow != null)
                    wrapper.Insert(0f, borderFadeWindow.Close(SequenceType.UnSequenced));

                if (playbackHandle != null)
                    wrapper.Insert(0f, playbackHandle.Open(SequenceType.UnSequenced));
            }
            else
            {
                if (measurementDisplayWindow != null)
                    wrapper.Insert(0f, measurementDisplayWindow.Close(SequenceType.UnSequenced));

                if (middleGraphicWindow != null)
                    wrapper.Insert(0.5f, middleGraphicWindow.Close(SequenceType.UnSequenced));

                if (audioVizWindow != null)
                    wrapper.Insert(0.5f, audioVizWindow.Close(SequenceType.UnSequenced));

                if (borderFadeWindow != null)
                    wrapper.Insert(0.9f, borderFadeWindow.Close(SequenceType.UnSequenced));

                if (playbackHandle != null)
                    wrapper.Insert(0.9f, playbackHandle.Open(SequenceType.UnSequenced));

                if (measurementDisplay != null)
                    wrapper.InsertCallback(0.9f, () => { measurementDisplay.UpdateDisplay(); });

                if (measurementDisplayWindow != null)
                    wrapper.Insert(0.9f, measurementDisplayWindow.Open(SequenceType.UnSequenced));
            }

            return wrapper;
        }

        private Tween SwitchToTrim()
        {
            Sequence wrapper = DOTween.Sequence();

            wrapper.InsertCallback(0f, () => {
                //Only pause here so we don't jump the slider around
                playbackManager.PauseAudio();
            });

            if (playbackHandle != null)
                wrapper.Insert(0f, playbackHandle.Close(SequenceType.UnSequenced));

            if (borderFadeWindow != null)
                wrapper.Insert(0f, borderFadeWindow.Open(SequenceType.UnSequenced));

            if (leftCoverWindow != null)
                wrapper.Insert(0f, leftCoverWindow.Close(SequenceType.UnSequenced));

            if (rightCoverWindow != null)
                wrapper.Insert(0f, rightCoverWindow.Close(SequenceType.UnSequenced));

            if (trimSliderIsUnTrimmed)
            {
                //Close out the rest of the trim graphics simultaneously with those above
                //Because the measurementDisplayWindow and middleGraphicWindow won't appear to change

                if (middleGraphicWindow != null)
                    wrapper.Insert(0f, middleGraphicWindow.Open(SequenceType.UnSequenced));

                if (audioVizWindow != null)
                    wrapper.Insert(0f, audioVizWindow.Open(SequenceType.UnSequenced));

                if (rightHandleWindow != null)
                    wrapper.Insert(0f, rightHandleWindow.Open(SequenceType.UnSequenced));

                if (leftHandleWindow != null)
                    wrapper.Insert(0f, leftHandleWindow.Open(SequenceType.UnSequenced));
            }
            else
            {
                //Progressively close out the rest of the trim graphics

                if (measurementDisplayWindow != null)
                    wrapper.Insert(0f, measurementDisplayWindow.Close(SequenceType.UnSequenced));

                if (middleGraphicWindow != null)
                    wrapper.Insert(0.5f, middleGraphicWindow.Open(SequenceType.UnSequenced));

                if (audioVizWindow != null)
                    wrapper.Insert(0.5f, audioVizWindow.Open(SequenceType.UnSequenced));

                if (rightHandleWindow != null)
                    wrapper.Insert(0.7f, rightHandleWindow.Open(SequenceType.UnSequenced));

                if (leftHandleWindow != null)
                    wrapper.Insert(0.7f, leftHandleWindow.Open(SequenceType.UnSequenced));

                if (measurementDisplay != null)
                    wrapper.InsertCallback(0.7f, () => { measurementDisplay.UpdateDisplay(); });

                if (measurementDisplayWindow != null)
                    wrapper.Insert(0.7f, measurementDisplayWindow.Open(SequenceType.UnSequenced));
            }

            return wrapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doSwitchToPlaybackMode"></param>
        public Tween Switch(bool doSwitchToPlaybackMode, bool doCancelTrimChanges = false, bool doCompleteImmediately = false)
        {
            if (playbackManager == null) return null;
            if (trimSlider == null) return null;
            if (sequenceManager == null) return null;

            isPlayback = doSwitchToPlaybackMode;

            sequenceManager.KillCurrentSequence();

            Sequence wrapper = DOTween.Sequence();

            if (doCompleteImmediately)
            {
                if (doCancelTrimChanges && !trimSliderIsUnchanged)
                {
                    // Temporarily SetValues to target so that
                    // positionTween in SliderMiddleGraphicWindow is properly set up
                    float auxMin = trimSlider.Values.minValue;
                    float auxMax = trimSlider.Values.maxValue;

                    // Before doing the switch tween, reset the slider
                    ResetTrim(true);
                    playbackManager.minValue = trimSlider.Values.minValue;
                    playbackManager.maxValue = trimSlider.Values.maxValue;

                    // Then switch to playback
                    Tween switchToPlaybackTween = SwitchToPlayback();
                    switchToPlaybackTween.Complete();

                    // Undo SetValues above
                    trimSlider.SetValues(auxMin, auxMax);
                }
                else
                {
                    playbackManager.minValue = trimSlider.Values.minValue;
                    playbackManager.maxValue = trimSlider.Values.maxValue;

                    Tween switchTween;
                    if (doSwitchToPlaybackMode)
                        switchTween = SwitchToPlayback();
                    else
                        switchTween = SwitchToTrim();
                    switchTween.Complete();
                }

                onSwitchComplete?.Invoke(doSwitchToPlaybackMode);

                return null;
            }
            else
            {
                wrapper.InsertCallback(0f, () => { onSwitchStart?.Invoke(doSwitchToPlaybackMode); });

                if (doCancelTrimChanges && !trimSliderIsUnchanged)
                {
                    // Before doing the switch tween, reset the slider
                    Tween resetTween = ResetTrim();
                    resetTween.OnComplete(() =>
                    {
                        playbackManager.minValue = trimSlider.Values.minValue;
                        playbackManager.maxValue = trimSlider.Values.maxValue;
                    });
                    wrapper.Join(resetTween);

                    // Temporarily SetValues to target so that
                    // positionTween in SliderMiddleGraphicWindow is properly set up
                    float auxMin = trimSlider.Values.minValue;
                    float auxMax = trimSlider.Values.maxValue;
                    trimSlider.SetValues(trimSliderCachedMin, trimSliderCachedMax);

                    // Then do switch tween after reset
                    wrapper.Append(SwitchToPlayback());
                    wrapper.AppendCallback(() => { onSwitchComplete?.Invoke(true); });

                    // Undo SetValues above
                    trimSlider.SetValues(auxMin, auxMax);
                }
                else
                {
                    playbackManager.minValue = trimSlider.Values.minValue;
                    playbackManager.maxValue = trimSlider.Values.maxValue;

                    if (doSwitchToPlaybackMode)
                        wrapper.Join(SwitchToPlayback());
                    else
                        wrapper.Join(SwitchToTrim());
                }

                wrapper.AppendCallback(() => { onSwitchComplete?.Invoke(doSwitchToPlaybackMode); });

                return wrapper;
            }

        }

        public void OnSwitch()
        {
            //Toggle
            Switch(!isPlayback);
        }
    }
}

