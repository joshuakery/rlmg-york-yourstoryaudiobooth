using DG.Tweening;
using JoshKery.GenericUI.DOTweenHelpers;
using System;
using TMPro;
using UnityEngine;

namespace JoshKery.York.AudioRecordingBooth
{
    public class TimerTextWindow : BaseWindow
    {
        /// <summary>
        /// When in the process RecordingDuration the countdown text should begin
        /// </summary>
        private int STARTCOUNTDOWNAT = 10;

        private RecordingSession sessionManager;

        private AudioRecorderProcess process;

        [SerializeField]
        private TMP_Text textDisplay;

        /// <summary>
        /// Set to true if, when session.State == RecordingNow, the text should update with the countdown
        /// </summary>
        private bool doCountdownTextInUpdate = false;

        /// <summary>
        /// Updated while session.State == RecordingNow
        /// so that a Pulse animation can be triggered when secondsRemaining equals STARTCOUNTDOWNAT
        /// </summary>
        private int countLastFrame = -1;

        /// <summary>
        /// The seconds remaining for the process, based on its RecordingDuration
        /// </summary>
        private int secondsRemaining
        {
            get
            {
                if (process == null) return -1;

                return Math.Max(
                    0,
                    Convert.ToInt32(
                        (process.RecordingDuration - ((TimeSpan)(DateTime.Now - process.StartTime)).TotalMilliseconds) / 1000f
                    )
                );
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (process == null)
                process = FindObjectOfType<AudioRecorderProcess>();

            if (sessionManager == null)
                sessionManager = FindObjectOfType<RecordingSession>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (process != null)
            {
                process.onFirstProcessStarted += OnFirstProcessStarted;
                process.onFail += OnFail;
                process.onRecordingError += OnRecordingError;
                //process.onRecordingSuccess += OnRecordingSuccess;
                process.onStopRequested += OnStopRequested;
                process.onInit += OnInit;
            }
        }


        protected override void OnDisable()
        {
            base.OnDisable();

            if (process != null)
            {
                process.onFirstProcessStarted -= OnFirstProcessStarted;
                process.onFail -= OnFail;
                process.onRecordingError -= OnRecordingError;
                process.onRecordingSuccess -= OnRecordingSuccess;
                process.onStopRequested -= OnStopRequested;
                process.onInit -= OnInit;
            }
        }

        private void OnFirstProcessStarted()
        {
            doCountdownTextInUpdate = false;
            PulseUnsequenced();
        }

        private void OnFail(System.Exception e)
        {
            doCountdownTextInUpdate = false;
            PulseUnsequenced();
        }

        

        private void OnStopRequested()
        {
            doCountdownTextInUpdate = false;
            PulseUnsequenced();
        }
        private void OnRecordingError()
        {
            doCountdownTextInUpdate = false;
            PulseUnsequenced();
        }

        private void OnRecordingSuccess()
        {
            doCountdownTextInUpdate = false;
            PulseUnsequenced();
        }

        private void OnInit()
        {
            doCountdownTextInUpdate = false;
            PulseUnsequenced();
        }

        private void Update()
        {
            if (sessionManager != null && process != null)
            {
                if (sessionManager.state == RecordingSession.State.RecordingNow)
                {
                    //Pulse animation if switching to countdown
                    if (secondsRemaining == STARTCOUNTDOWNAT && countLastFrame != STARTCOUNTDOWNAT)
                        PulseUnsequenced();

                    //Update countLastFrame
                    countLastFrame = secondsRemaining;

                    //If ever it's the wrong time to countdown, do not display
                    if (secondsRemaining > STARTCOUNTDOWNAT)
                        doCountdownTextInUpdate = false;

                    //Change text display
                    if (textDisplay != null)
                    {
                        if (doCountdownTextInUpdate)
                        {
                            textDisplay.text = string.Format("{0} seconds left", secondsRemaining);
                        }
                    }

                }
            }
        }

        public void PulseUnsequenced()
        {
            _PulseUnsequenced();
        }

        private void _PulseUnsequenced()
        {
            Sequence overallWrapper = DOTween.Sequence();

            Sequence closeTweenWrapper = DOTween.Sequence();

            Tween closeTween = _WindowAction(closeSequence, SequenceType.UnSequenced);

            if (closeTween != null)
            {
                closeTweenWrapper.Join(closeTween);

                closeTweenWrapper.onComplete = () =>
                {
                    OnPulseMiddle();
                };

                overallWrapper.Join(closeTweenWrapper);
            }

            Tween openTween = _WindowAction(openSequence, SequenceType.UnSequenced);

            if (openTween != null)
                overallWrapper.Append(openTween);
        }

        private void OnPulseMiddle()
        {
            if (textDisplay != null)
            {
                if (sessionManager != null)
                {
                    switch (sessionManager.state)
                    {
                        case (RecordingSession.State.NotYetStarted):
                            textDisplay.text = "Press Record to Begin";
                            break;
                        case (RecordingSession.State.RecordingNow):
                            textDisplay.text = string.Format(
                                "Speak for {0} seconds or less",
                                Mathf.RoundToInt(process.RecordingDuration / 1000f)
                            );
                            //Update may override this text if it's time to countdown
                            doCountdownTextInUpdate = (secondsRemaining <= STARTCOUNTDOWNAT);
                            break;
                        case (RecordingSession.State.CompletedRecording):
                        case (RecordingSession.State.SuccessfulRecording):
                            textDisplay.text = "Great!";
                            break;
                        case RecordingSession.State.Error:
                            textDisplay.text = "Oops! Something went wrong. Please try again.";
                            break;
                        default:
                            textDisplay.text = "";
                            break;
                    }
                    
                }
            }
        }
    }
}


