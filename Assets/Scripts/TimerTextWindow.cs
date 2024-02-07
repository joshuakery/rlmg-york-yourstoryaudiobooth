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


        private string PRESSRECORDTOBEGINTEXT = "Press Record\nto Begin";

        private string RECORDINGNOWTEMPLATE = "Speak for {0}\nseconds or less";

        private string COUNTDOWNTEMPLATE = "<mspace=0.65em>{0}</mspace> seconds left";

        private string SUCCESSFULRECORDINGTEXT = "Great!";

        private string ERRORTEXT = "Oops!\nSomething went wrong.\nPlease try again.";


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
            Pulse();
        }

        private void OnFail(System.Exception e)
        {
            doCountdownTextInUpdate = false;
            Pulse();
        }

        

        private void OnStopRequested()
        {
            doCountdownTextInUpdate = false;
            Pulse();
        }
        private void OnRecordingError()
        {
            doCountdownTextInUpdate = false;
            Pulse();
        }

        private void OnRecordingSuccess()
        {
            doCountdownTextInUpdate = false;
            Pulse();
        }

        private void OnInit()
        {
            doCountdownTextInUpdate = false;
            Pulse(SequenceType.CompleteImmediately);

            //Do this afterward, as the SessionManager hasn't necessarily updated yet with the Init
            textDisplay.text = PRESSRECORDTOBEGINTEXT;
        }

        private void Update()
        {
            if (sessionManager != null && process != null)
            {
                if (sessionManager.state == RecordingSession.State.RecordingNow)
                {
                    //Pulse animation if switching to countdown
                    if (secondsRemaining == STARTCOUNTDOWNAT && countLastFrame != STARTCOUNTDOWNAT)
                        Pulse();

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
                            textDisplay.text = string.Format(
                                COUNTDOWNTEMPLATE,
                                secondsRemaining >= 10 ? secondsRemaining : " " + secondsRemaining
                            );
                        }
                    }

                }
            }
        }



        public void Pulse(SequenceType sequenceType = SequenceType.UnSequenced)
        {
            _Pulse(sequenceType);
        }

        private void _Pulse(SequenceType sequenceType = SequenceType.UnSequenced)
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

            if (sequenceType == SequenceType.CompleteImmediately)
                overallWrapper.Complete();

            //todo attach to main sequence
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
                            textDisplay.text = PRESSRECORDTOBEGINTEXT;
                            break;
                        case (RecordingSession.State.RecordingNow):
                            textDisplay.text = string.Format(
                                RECORDINGNOWTEMPLATE,
                                Mathf.RoundToInt(process.RecordingDuration / 1000f)
                            );
                            //Update may override this text if it's time to countdown
                            doCountdownTextInUpdate = (secondsRemaining <= STARTCOUNTDOWNAT);
                            break;
                        case (RecordingSession.State.CompletedRecording):
                        case (RecordingSession.State.SuccessfulRecording):
                            textDisplay.text = SUCCESSFULRECORDINGTEXT;
                            break;
                        case RecordingSession.State.Error:
                            textDisplay.text = ERRORTEXT;
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


